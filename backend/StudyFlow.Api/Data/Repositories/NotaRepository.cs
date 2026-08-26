
using Microsoft.EntityFrameworkCore;
using StudyFlow.Api.Domain.Entities;
using StudyFlow.Api.Domain.Interfaces.Notas;

namespace StudyFlow.Api.Data.Repositories
{
    public class NotaRepository : INotaRepository
    {
        private readonly AppDbContext _context;

        public NotaRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task CriarAsync(Nota nota)
        {
            await _context.Notas.AddAsync(nota);
        }

        public void Atualizar(Nota nota)
        {
             _context.Notas.Update(nota);
        }

        public void Deletar(Nota nota)
        {
            _context.Notas.Remove(nota);
        }

        public async Task<IEnumerable<Nota>> ListarTodasAsync()
        {
            return await _context.Notas
            .Include(n => n.Tema)
            .AsNoTracking()
            .ToListAsync();
        }

        public async Task<Nota?> ObterPorIdAsync(int id)
        {
            return await _context.Notas
            .Include(n => n.Tema)
            .FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task<IEnumerable<Nota>> ObterPorTemaIdAsync(int temaId)
        {
            return await _context.Notas
            .Where(n => n.TemaId == temaId)
            .Include(n => n.Tema)
            .AsNoTracking()
            .ToListAsync();
        }

        public async Task<bool> SalvarAlteracoesAsync()
        {
            return await _context.SaveChangesAsync() > 0; 
        }
    }
}