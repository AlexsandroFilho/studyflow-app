using StudyFlow.Api.Domain.Interfaces.Rag;
using StudyFlow.Api.DTOs;
using StudyFlow.Api.Mappers;

namespace StudyFlow.Api.Services;

public sealed class ResumoTemaService(
    IResumoTemaRepository resumoTemaRepository,
    IContextoTemaService contextoTemaService,
    IBuscaContextoAnatomia buscaContextoAnatomia,
    IResumidorTemaAnatomia resumidor,
    IConfiguration configuration) : IResumoTemaService
{
    public async Task<ResumoTemaResponseDto> CriarAsync(int temaId, Guid usuarioId, CancellationToken cancellationToken = default)
    {
        var tema = await contextoTemaService.ObterAsync(temaId, usuarioId, cancellationToken)
            ?? throw new KeyNotFoundException("Tema não encontrado.");

        if (tema.Notas.Count == 0)
            throw new InvalidOperationException("Crie ao menos uma nota no tema antes de gerar um resumo.");

        var quantidadeChunks = configuration.GetSection("Ai").GetValue<int?>("ContextoQuantidadeChunksTema")
            ?? configuration.GetSection("Ai").GetValue<int?>("ContextoQuantidadeChunks")
            ?? 6;
        var evidencias = await buscaContextoAnatomia.BuscarAsync(MontarConsulta(tema), quantidadeChunks, cancellationToken);
        var resultado = await resumidor.ResumirAsync(tema, evidencias, cancellationToken);

        var resumo = resultado.ToEntity(temaId, usuarioId, resumidor.Modelo);
        await resumoTemaRepository.AdicionarAsync(resumo, cancellationToken);
        await resumoTemaRepository.SalvarAlteracoesAsync(cancellationToken);
        return resumo.ToResponseDto();
    }

    public async Task<IReadOnlyList<ResumoTemaResponseDto>> ListarAsync(int temaId, Guid usuarioId, CancellationToken cancellationToken = default)
    {
        var tema = await contextoTemaService.ObterAsync(temaId, usuarioId, cancellationToken);
        if (tema is null)
            throw new KeyNotFoundException("Tema não encontrado.");

        var resumos = await resumoTemaRepository.ListarPorTemaEUsuarioAsync(temaId, usuarioId, cancellationToken);
        return resumos.Select(x => x.ToResponseDto()).ToList();
    }

    private static string MontarConsulta(ContextoTemaDto tema)
    {
        var partes = new List<string> { $"Tema: {tema.Nome}" };
        if (!string.IsNullOrWhiteSpace(tema.Descricao))
            partes.Add(tema.Descricao);
        partes.AddRange(tema.Notas.Select(nota => $"Nota: {nota.Titulo}\n{nota.Conteudo}"));
        partes.AddRange(tema.Conexoes.Select(conexao =>
            $"Relação visual: {conexao.TituloOrigem} -> {conexao.TituloDestino}. Rótulo: {conexao.Rotulo ?? "não informado"}."));
        return string.Join("\n\n", partes);
    }
}
