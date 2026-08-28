
using StudyFlow.Api.Domain.Entities;
using StudyFlow.Api.DTOs;

namespace StudyFlow.Api.Mappers
{
    public static class TemaMapper
    {
        public static TemaResponseDto toResponseDto(this Tema tema)
        {
            return new TemaResponseDto(
                tema.Id,
                tema.Nome ?? string.Empty,
                tema.Descricao,
                tema.DataCriacao
            );
        }

        public static Tema toEntity(this CreateTemaDto dto, Guid usuarioId)
        {
            return criarNovoTema(dto.Nome, dto.Descricao, usuarioId);
        }

        public static Tema criarNovoTema(string nome, string? descricao = null, Guid? usuarioId = null)
        {
            return new Tema
            {
                Nome = nome.Trim(),
                Descricao = descricao?.Trim(),
                UsuarioId = usuarioId ?? Guid.Empty,
                DataCriacao = DateTime.UtcNow
            };
        }
    }
}