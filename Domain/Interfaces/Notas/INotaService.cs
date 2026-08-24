using StudyFlow.Api.DTOs;

namespace StudyFlow.Api.Domain.Interfaces.Notas
{
    public interface INotaService
    {
        Task<IEnumerable<NotaResponseDto>> ListarTodasAsync();
        Task<IEnumerable<NotaResponseDto>> ObterPorTemaIdAsync(int temaId);
        Task<NotaResponseDto?> ObterPorIdAsync(int id);
        Task<NotaResponseDto?> CriarAsync(CreateNotaDto dto);
        Task<bool> AtualizarAsync(int id, UpdateNotaDto dto);
        Task<bool> DeletarAsync(int id);
    }
}