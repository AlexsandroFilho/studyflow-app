using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudyFlow.Api.Domain.Entities;
using StudyFlow.Api.DTOs;

namespace StudyFlow.Api.Mappers
{
    public static class NotaMapper
    {
        public static NotaResponseDto toResponseDto(this Nota nota)
        {
            return new NotaResponseDto(
                nota.Id,
                nota.Titulo ?? string.Empty,
                nota.Conteudo ?? string.Empty,
                nota.ResumoIA,
                nota.DataCriacao,
                nota.TemaId,
                nota.Tema?.Nome
            );
        }

        public static Nota toEntity(this CreateNotaDto dto)
        {
            return new Nota
            {
                Titulo = dto.Titulo,
                Conteudo = dto.Conteudo,
                ResumoIA = dto.ResumoIa,
                TemaId = dto.TemaId,
                DataCriacao = DateTime.UtcNow
            };
        }
    }
}