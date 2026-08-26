using Microsoft.EntityFrameworkCore;
using StudyFlow.Api.Domain.Entities;
using StudyFlow.Api.Domain.Interfaces.Temas;

namespace StudyFlow.Api.Data.Repositories
{
    public class TemaRepository : ITemaRepository
    {

        private readonly AppDbContext _context;

        public TemaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Tema>> ListarTodosAsync()
        {
            return await _context.Temas
            .AsNoTracking()
            .ToListAsync();
        }

        public async Task<Tema?> ObterPorIdAsync(int id)
        {
            return await _context.Temas
            .Include(t => t.Notas)
            .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task CriarAsync(Tema tema)
        {
            await _context.Temas.AddAsync(tema);
        }

        public void Atualizar(Tema tema)
        {
            _context.Temas.Update(tema);
        }

        public void Deletar(Tema tema)
        {
            _context.Temas.Remove(tema);
        }

        public async Task<bool> SalvarAlteracoesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }


        
    }
}