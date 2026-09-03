using StudyFlow.Api.Domain.Enums;
using StudyFlow.Api.Domain.Interfaces.Rag;
using StudyFlow.Api.DTOs;
using StudyFlow.Api.Mappers;

namespace StudyFlow.Api.Services;

public sealed class AdministracaoFonteAnatomiaService(
    IIngestaoFonteAnatomiaRepository ingestaoRepository,
    IIngestaoAnatomiaService ingestaoService,
    IArmazenamentoFonteAnatomia armazenamento,
    ILogger<AdministracaoFonteAnatomiaService> logger) : IAdministracaoFonteAnatomiaService
{
    private const long TamanhoMaximoArquivo = 25L * 1024 * 1024;

    public async Task<IngestaoFonteAnatomiaResponseDto> SolicitarAsync(CriarIngestaoFonteAnatomiaRequest request, Guid usuarioId, CancellationToken cancellationToken = default)
    {
        ValidarSolicitacao(request);
        var id = Guid.NewGuid();
        var chave = await armazenamento.ArmazenarTemporarioAsync(request.Arquivo, request.NomeArquivo, id, cancellationToken);
        var ingestao = request.ToEntity(usuarioId, chave);
        ingestao.Id = id;
        await ingestaoRepository.AdicionarAsync(ingestao, cancellationToken);
        await ingestaoRepository.SalvarAlteracoesAsync(cancellationToken);
        return ingestao.ToResponse();
    }

    public async Task<IReadOnlyList<IngestaoFonteAnatomiaResponseDto>> ListarAsync(CancellationToken cancellationToken = default) =>
        (await ingestaoRepository.ListarAsync(cancellationToken)).Select(x => x.ToResponse()).ToList();

    public async Task<IngestaoFonteAnatomiaResponseDto> ReprocessarAsync(Guid ingestaoId, CancellationToken cancellationToken = default)
    {
        var ingestao = await ingestaoRepository.ObterPorIdAsync(ingestaoId, cancellationToken)
            ?? throw new KeyNotFoundException("Ingestão não encontrada.");
        if (ingestao.Status != StatusIngestaoFonteAnatomia.Falhou)
            throw new InvalidOperationException("Apenas ingestões com falha podem ser reprocessadas.");

        ingestao.Reenfileirar();
        await ingestaoRepository.SalvarAlteracoesAsync(cancellationToken);
        return ingestao.ToResponse();
    }

    public async Task ReenfileirarInterrompidasAsync(CancellationToken cancellationToken = default) =>
        await ingestaoRepository.ReenfileirarProcessamentosInterrompidosAsync(cancellationToken);

    public async Task<bool> ProcessarProximaAsync(CancellationToken cancellationToken = default)
    {
        var ingestao = await ingestaoRepository.ObterProximaPendenteAsync(cancellationToken);
        if (ingestao is null) return false;

        ingestao.Iniciar();
        await ingestaoRepository.SalvarAlteracoesAsync(cancellationToken);

        string? caminhoTemporario = null;
        try
        {
            caminhoTemporario = await armazenamento.BaixarParaArquivoTemporarioAsync(ingestao.ArquivoTemporarioChave, cancellationToken);
            var resultado = await ingestaoService.IngerirAsync(new FonteIngestaoRequest(
                caminhoTemporario,
                ingestao.Titulo,
                ingestao.Autor,
                ingestao.Versao,
                ingestao.Assunto,
                ingestao.Subassunto), cancellationToken);

            ingestao.Concluir(resultado.FonteId, resultado.QuantidadeChunks);
            await ingestaoRepository.SalvarAlteracoesAsync(cancellationToken);
            await armazenamento.RemoverAsync(ingestao.ArquivoTemporarioChave, cancellationToken);
            logger.LogInformation("Ingestão administrativa {IngestaoId} concluída.", ingestao.Id);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Falha na ingestão administrativa {IngestaoId}.", ingestao.Id);
            ingestao.Falhar(ObterMensagemSegura(exception));
            await ingestaoRepository.SalvarAlteracoesAsync(CancellationToken.None);
        }
        finally
        {
            if (caminhoTemporario is not null && File.Exists(caminhoTemporario) && caminhoTemporario.StartsWith(Path.GetTempPath(), StringComparison.OrdinalIgnoreCase))
                File.Delete(caminhoTemporario);
        }

        return true;
    }

    private static void ValidarSolicitacao(CriarIngestaoFonteAnatomiaRequest request)
    {
        if (!request.NomeArquivo.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Envie um arquivo PDF.");
        if (request.Arquivo.CanSeek && request.Arquivo.Length > TamanhoMaximoArquivo)
            throw new InvalidOperationException("O PDF deve ter no máximo 25 MB.");
        if (string.IsNullOrWhiteSpace(request.Titulo) || string.IsNullOrWhiteSpace(request.Versao) || string.IsNullOrWhiteSpace(request.Assunto))
            throw new InvalidOperationException("Título, versão e assunto são obrigatórios.");
    }

    private static string ObterMensagemSegura(Exception exception) => exception switch
    {
        FileNotFoundException => "O arquivo temporário não foi encontrado. Envie o PDF novamente.",
        InvalidOperationException => exception.Message,
        _ => "Não foi possível concluir a ingestão. Revise o PDF e tente reprocessar mais tarde."
    };
}
