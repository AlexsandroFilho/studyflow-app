using System.Security.Cryptography;
using StudyFlow.Api.Configurations;
using StudyFlow.Api.Domain.Entities;
using StudyFlow.Api.Domain.Enums;
using StudyFlow.Api.Domain.Interfaces.Rag;
using StudyFlow.Api.DTOs;
using StudyFlow.Api.Mappers;
using UglyToad.PdfPig;

namespace StudyFlow.Api.Services;

public sealed class IngestaoAnatomiaService(
    IFonteAnatomiaRepository fonteRepository,
    IEmbeddingService embeddingService,
    IArmazenamentoFonteAnatomia armazenamento,
    IConfiguration configuration,
    ILogger<IngestaoAnatomiaService> logger) : IIngestaoAnatomiaService
{
    public async Task<FonteIngestaoResultado> IngerirAsync(FonteIngestaoRequest request, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(request.CaminhoPdf))
            throw new FileNotFoundException("O PDF informado não foi encontrado.", request.CaminhoPdf);

        var hash = await CalcularHashAsync(request.CaminhoPdf, cancellationToken);
        var fonteExistente = await fonteRepository.ObterPorHashComChunksAsync(hash, cancellationToken);

        var chave = await armazenamento.ArmazenarAsync(request.CaminhoPdf, hash, cancellationToken);
        var fonte = fonteExistente ?? request.ToEntity(chave, hash);
        var reindexada = fonteExistente is not null;

        if (fonteExistente is null)
        {
            await fonteRepository.AdicionarAsync(fonte, cancellationToken);
            await fonteRepository.SalvarAlteracoesAsync(cancellationToken);
        }
        else
        {
            fonte.Atualizar(request, chave);
            if (fonte.Publicada)
            {
                fonteRepository.RemoverChunks(fonteExistente.Chunks);
                fonte.Chunks.Clear();
                await fonteRepository.SalvarAlteracoesAsync(cancellationToken);
            }
        }

        var chunks = ExtrairChunks(request);
        var chunksPersistidos = fonte.Chunks
            .Select(x => CriarIdentificadorChunk(x.Pagina, x.Texto))
            .ToHashSet();
        var tamanhoLote = ObterTamanhoLote();
        var pendentesNoLote = 0;
        var novosChunks = new List<AnatomiaChunkVector>(tamanhoLote);

        foreach (var chunk in chunks)
        {
            if (!chunksPersistidos.Add(CriarIdentificadorChunk(chunk.Pagina, chunk.Texto)))
                continue;

            var embedding = await embeddingService.GerarAsync(chunk.Texto, TipoTarefaEmbedding.Documento, cancellationToken);
            var novoChunk = chunk.ToChunkEntity(request, embedding);
            novoChunk.FonteAnatomiaId = fonte.Id;
            novosChunks.Add(novoChunk);
            pendentesNoLote++;

            if (pendentesNoLote < tamanhoLote)
                continue;

            await fonteRepository.AdicionarChunksAsync(novosChunks, cancellationToken);
            await fonteRepository.SalvarAlteracoesAsync(cancellationToken);
            logger.LogInformation("Lote de embeddings persistido para a fonte {FonteId}.", fonte.Id);
            pendentesNoLote = 0;
            novosChunks.Clear();
        }

        if (pendentesNoLote > 0)
        {
            await fonteRepository.AdicionarChunksAsync(novosChunks, cancellationToken);
            await fonteRepository.SalvarAlteracoesAsync(cancellationToken);
            logger.LogInformation("Último lote de embeddings persistido para a fonte {FonteId}.", fonte.Id);
        }

        fonte.Publicada = true;
        await fonteRepository.SalvarAlteracoesAsync(cancellationToken);
        return new FonteIngestaoResultado(fonte.Id, chunks.Count, reindexada);
    }

    private int ObterTamanhoLote()
    {
        var tamanhoConfigurado = configuration.GetSection(AiSettings.SectionName).GetValue<int>("TamanhoLoteIngestao");
        return tamanhoConfigurado > 0 ? tamanhoConfigurado : 20;
    }

    private static string CriarIdentificadorChunk(int pagina, string texto) => $"{pagina}:{texto}";

    private static List<ChunkAnatomiaDto> ExtrairChunks(FonteIngestaoRequest request)
    {
        var resultado = new List<ChunkAnatomiaDto>();
        using var pdf = PdfDocument.Open(request.CaminhoPdf);
        foreach (var pagina in pdf.GetPages())
        {
            var texto = pagina.Text?.Trim();
            if (string.IsNullOrWhiteSpace(texto))
                continue;

            resultado.AddRange(DividirTexto(texto, pagina.Number, request.Assunto));
        }

        if (resultado.Count == 0)
            throw new InvalidOperationException("O PDF não contém texto extraível. Use um PDF pesquisável.");
        return resultado;
    }

    private static IEnumerable<ChunkAnatomiaDto> DividirTexto(string texto, int pagina, string? secao)
    {
        const int tamanhoMaximo = 1800;
        const int sobreposicao = 250;
        for (var inicio = 0; inicio < texto.Length; inicio += tamanhoMaximo - sobreposicao)
        {
            var tamanho = Math.Min(tamanhoMaximo, texto.Length - inicio);
            var fim = inicio + tamanho;
            if (fim < texto.Length)
            {
                var quebra = texto.LastIndexOfAny(['.', '!', '?', '\n'], fim - 1, tamanho);
                if (quebra > inicio + 500)
                    fim = quebra + 1;
            }
            var parte = texto[inicio..fim].Trim();
            if (!string.IsNullOrWhiteSpace(parte))
                yield return new ChunkAnatomiaDto(parte, pagina, secao);
            if (fim >= texto.Length)
                yield break;
            inicio = fim - (tamanhoMaximo - sobreposicao);
        }
    }

    private static async Task<string> CalcularHashAsync(string caminho, CancellationToken cancellationToken)
    {
        await using var stream = File.OpenRead(caminho);
        var hash = await SHA256.HashDataAsync(stream, cancellationToken);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
