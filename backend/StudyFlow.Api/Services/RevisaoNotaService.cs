using StudyFlow.Api.Configurations;
using StudyFlow.Api.Domain.Interfaces.Notas;
using StudyFlow.Api.Domain.Interfaces.Rag;
using StudyFlow.Api.DTOs;
using StudyFlow.Api.Mappers;

namespace StudyFlow.Api.Services;

public sealed class RevisaoNotaService(
    INotaRepository notaRepository,
    IRevisaoNotaRepository revisaoRepository,
    IContextoGrafoNotasService contextoGrafo,
    IBuscaContextoAnatomia buscaContexto,
    IRevisorAnatomia revisor,
    IConfiguration configuration) : IRevisaoNotaService
{
    public async Task<RevisaoNotaResponseDto> CriarAsync(int notaId, Guid usuarioId, CancellationToken cancellationToken = default)
    {
        var nota = await contextoGrafo.ObterAsync(notaId, usuarioId, cancellationToken)
            ?? throw new KeyNotFoundException("Nota não encontrada.");

        var consulta = string.Join("\n", new[] { nota.Titulo, nota.Conteudo }.Concat(nota.Conexoes.Select(x => $"{x.Titulo}\n{x.Conteudo}")));
        var quantidade = configuration.GetSection(AiSettings.SectionName).GetValue<int?>("ContextoQuantidadeChunks") ?? 6;
        var evidencias = await buscaContexto.BuscarAsync(consulta, quantidade, cancellationToken);
        var resultado = await revisor.RevisarAsync(nota, evidencias, cancellationToken);

        var revisao = resultado.ToEntity(notaId, usuarioId, revisor.Modelo);
        await revisaoRepository.AdicionarAsync(revisao, cancellationToken);
        await revisaoRepository.SalvarAlteracoesAsync(cancellationToken);
        return revisao.ToResponseDto();
    }

    public async Task<IReadOnlyList<RevisaoNotaResponseDto>> ListarAsync(int notaId, Guid usuarioId, CancellationToken cancellationToken = default)
    {
        var nota = await notaRepository.ObterPorIdAsync(notaId);
        if (nota is null || nota.UsuarioId != usuarioId)
            throw new KeyNotFoundException("Nota não encontrada.");

        var revisoes = await revisaoRepository.ListarPorNotaEUsuarioAsync(notaId, usuarioId, cancellationToken);
        return revisoes.Select(x => x.ToResponseDto()).ToList();
    }
}
