using Microsoft.EntityFrameworkCore;
using StudyFlow.Api.Data;
using StudyFlow.Api.Domain.Entities;
using StudyFlow.Api.Domain.Interfaces.Usuarios;

namespace StudyFlow.Api.Data.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AppDbContext _context;

        public UsuarioRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario?> ObterPorEmailAsync(string email)
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<Usuario?> ObterPorIdAsync(Guid id)
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task CriarAsync(Usuario usuario)
        {
            await _context.Usuarios.AddAsync(usuario);
        }

        public async Task<bool> SalvarAlteracoesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
