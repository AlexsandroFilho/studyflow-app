
using StudyFlow.Api.DTOs;

namespace StudyFlow.Api.Domain.Interfaces.Temas
{
    public interface ITemaService
    {
        Task<IEnumerable<TemaResponseDto>> ListarTodosAsync();
        Task<TemaResponseDto?> ObterPorIdAsync(int id);
        Task<TemaResponseDto> CriarAsync(CreateTemaDto dto);
        Task<bool> AtualizarAsync(int id, UpdateTemaDto dto);
        Task<bool> DeletarAsync(int id);
    }
}