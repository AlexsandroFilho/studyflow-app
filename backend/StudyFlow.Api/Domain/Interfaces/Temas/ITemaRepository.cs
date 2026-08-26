using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudyFlow.Api.Domain.Entities;

namespace StudyFlow.Api.Domain.Interfaces.Temas
{
    public interface ITemaRepository
    {
        Task<IEnumerable<Tema>> ListarTodosAsync();
        Task<Tema?> ObterPorIdAsync(int id);
        Task CriarAsync(Tema tema);
        void Atualizar(Tema tema);
        void Deletar(Tema tema);
        Task<bool> SalvarAlteracoesAsync();

    }
}