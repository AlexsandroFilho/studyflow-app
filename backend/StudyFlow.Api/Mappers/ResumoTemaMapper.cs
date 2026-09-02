using System.Text.Json;
using StudyFlow.Api.Domain.Entities;
using StudyFlow.Api.DTOs;

namespace StudyFlow.Api.Mappers;

public static class ResumoTemaMapper
{
    public static ResumoTema ToEntity(this ResultadoResumoTemaDto resultado, int temaId, Guid usuarioId, string modelo) => new()
    {
        TemaId = temaId,
        UsuarioId = usuarioId,
        Status = resultado.Status,
        ResultadoJson = JsonSerializer.Serialize(resultado),
        Modelo = modelo
    };

    public static ResumoTemaResponseDto ToResponseDto(this ResumoTema resumo) => new(
        resumo.Id,
        resumo.TemaId,
        JsonSerializer.Deserialize<ResultadoResumoTemaDto>(resumo.ResultadoJson)
            ?? throw new InvalidOperationException("Histórico de resumo inválido."),
        resumo.Modelo,
        resumo.DataCriacao);
}
