using StudyFlow.Api.Domain.Entities;
using StudyFlow.Api.DTOs;

namespace StudyFlow.Api.Mappers;

public static class ContextoNotaMapper
{
    public static ContextoNotaDto ToContextoDto(this Nota nota, IReadOnlyList<ContextoNotaConectadaDto> conexoes) => new(
        nota.Id,
        nota.Titulo ?? "Sem título",
        nota.Conteudo ?? string.Empty,
        conexoes);

    public static ContextoNotaConectadaDto ToContextoConectadoDto(this Nota nota, string? rotulo) => new(
        nota.Id,
        nota.Titulo ?? "Sem título",
        nota.Conteudo ?? string.Empty,
        rotulo);
}
