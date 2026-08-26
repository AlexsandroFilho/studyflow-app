using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudyFlow.Api.Domain.Entities;

namespace StudyFlow.Api.Domain.Interfaces.Conexao
{
    public interface IConexaoNotaRepository
    {
        Task<IEnumerable<ConexaoNota>> ObterTodasAsync();
        Task<IEnumerable<ConexaoNota>> ObterPorTemaIdAsync(int temaId);
        Task<IEnumerable<ConexaoNota>> ObterPorNotaIdAsync(int notaId);
        Task<ConexaoNota?> ObterPorIdAsync(int id);
        Task<ConexaoNota?> ObterPorParAsync(int origemId, int destinoId);
        Task AdicionarAsync(ConexaoNota conexao);
        void Remover(ConexaoNota conexao);
        Task<bool> SalvarAlteracoesAsync();
    }
}