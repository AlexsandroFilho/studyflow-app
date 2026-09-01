using System.Text.Json;
using StudyFlow.Api.Domain.Entities;
using StudyFlow.Api.DTOs;

namespace StudyFlow.Api.Mappers;

public static class RevisaoNotaMapper
{
    public static RevisaoNota ToEntity(this ResultadoRevisaoNotaDto resultado, int notaId, Guid usuarioId, string modelo) => new()
    {
        NotaId = notaId,
        UsuarioId = usuarioId,
        Status = resultado.Status,
        ResultadoJson = JsonSerializer.Serialize(resultado),
        Modelo = modelo
    };

    public static RevisaoNotaResponseDto ToResponseDto(this RevisaoNota revisao) => new(
        revisao.Id,
        revisao.NotaId,
        JsonSerializer.Deserialize<ResultadoRevisaoNotaDto>(revisao.ResultadoJson)
            ?? throw new InvalidOperationException("Histórico de revisão inválido."),
        revisao.Modelo,
        revisao.DataCriacao);
}
