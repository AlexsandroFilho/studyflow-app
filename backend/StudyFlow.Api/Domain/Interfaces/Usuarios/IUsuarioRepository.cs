using StudyFlow.Api.Domain.Entities;

namespace StudyFlow.Api.Domain.Interfaces.Usuarios
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> ObterPorEmailAsync(string email);
        Task<Usuario?> ObterPorIdAsync(Guid id);
        Task CriarAsync(Usuario usuario);
        Task<bool> SalvarAlteracoesAsync();
    }
}
