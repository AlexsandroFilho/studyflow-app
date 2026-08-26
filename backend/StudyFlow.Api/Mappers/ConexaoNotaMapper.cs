
using StudyFlow.Api.Domain.Entities;
using StudyFlow.Api.DTOs;

namespace StudyFlow.Api.Mappers
{
    public static class ConexaoNotaMapper
    {
        public static ConexaoResponseDto toResponseDto(this ConexaoNota conexao)
        {
            return new ConexaoResponseDto(
                conexao.Id,
                conexao.NotaOrigemId,
                conexao.NotaDestinoId,
                conexao.Rotulo,
                conexao.DataCriacao
            );
        }

        public static ConexaoNota toEntity(this CreateConexaoDto dto)
        {
            return new ConexaoNota
            {
                NotaOrigemId = dto.NotaOrigemId,
                NotaDestinoId = dto.NotaDestinoId,
                Rotulo = dto.Rotulo?.Trim(),
                DataCriacao = DateTime.UtcNow
            };
        }

        public static ConexaoNota criarNovaConexao(int origemId, int destinoId, string? rotulo = null)
        {
            return new ConexaoNota
            {
                NotaOrigemId = origemId,
                NotaDestinoId = destinoId,
                Rotulo = rotulo?.Trim(),
                DataCriacao = DateTime.UtcNow
            };
        }
    }
}