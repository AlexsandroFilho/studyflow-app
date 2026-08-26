using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudyFlow.Api.DTOs;

namespace StudyFlow.Api.Domain.Interfaces.Conexao
{
    public interface IConexaoNotaService
    {
        Task<IEnumerable<ConexaoResponseDto>> ListarTodasAsync(int? temaId = null);
        Task<IEnumerable<ConexaoResponseDto>> ObterPorNotaIdAsync(int notaId);
        Task<ConexaoResponseDto?> ObterPorIdAsync(int id);
        Task<ConexaoResponseDto?> CriarConexaoAsync(CreateConexaoDto dto);
        Task<bool> DeletarPorIdAsync(int id);
        Task<bool> DeletarPorParAsync(int origemId, int destinoId);
    }
}