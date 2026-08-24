using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudyFlow.Api.Domain.Entities;

namespace StudyFlow.Api.Domain.Interfaces.Notas
{
    public interface INotaRepository
    {
        Task<IEnumerable<Nota>> ListarTodasAsync();
        Task<IEnumerable<Nota>> ObterPorTemaIdAsync(int temaId);
        Task<Nota?> ObterPorIdAsync(int id);
        Task CriarAsync(Nota nota);
        void Atualizar(Nota nota);
        void Deletar(Nota nota);
        Task<bool> SalvarAlteracoesAsync();
    }
}