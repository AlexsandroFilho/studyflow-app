using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudyFlow.Api.DTOs
{
    public record CreateTemaDto(string Nome, string? Descricao);
    public record UpdateTemaDto(string Nome, string? Descricao);
    public record TemaResponseDto(int Id, string Nome, string? Descricao, DateTime DataCriacao);
}