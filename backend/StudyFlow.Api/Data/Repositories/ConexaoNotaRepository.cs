

using Microsoft.EntityFrameworkCore;
using StudyFlow.Api.Domain.Entities;
using StudyFlow.Api.Domain.Interfaces.Conexao;

namespace StudyFlow.Api.Data.Repositories
{
    public class ConexaoNotaRepository : IConexaoNotaRepository
    {
        private readonly AppDbContext _context;

        public ConexaoNotaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ConexaoNota>> ObterTodasAsync()
        {
            return await _context.ConexaoNotas
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<ConexaoNota>> ObterPorTemaIdAsync(int temaId)
        {
            return await _context.ConexaoNotas
                .Include(c => c.NotaOrigem)
                .Include(c => c.NotaDestino)
                .Where(c => c.NotaOrigem!.TemaId == temaId || c.NotaDestino!.TemaId == temaId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<ConexaoNota>> ObterPorNotaIdAsync(int notaId)
        {
            return await _context.ConexaoNotas
                .Where(c => c.NotaOrigemId == notaId || c.NotaDestinoId == notaId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ConexaoNota?> ObterPorIdAsync(int id)
        {
            return await _context.ConexaoNotas
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<ConexaoNota?> ObterPorParAsync(int origemId, int destinoId)
        {
            return await _context.ConexaoNotas
                .FirstOrDefaultAsync(c => c.NotaOrigemId == origemId && c.NotaDestinoId == destinoId);
        }

        public async Task AdicionarAsync(ConexaoNota conexao)
        {
            await _context.ConexaoNotas.AddAsync(conexao);
        }

        public void Remover(ConexaoNota conexao)
        {
            _context.ConexaoNotas.Remove(conexao);
        }

        public async Task<bool> SalvarAlteracoesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}