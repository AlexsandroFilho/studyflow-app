

using StudyFlow.Api.Domain.Entities;
using StudyFlow.Api.Domain.Interfaces.Conexao;
using StudyFlow.Api.Domain.Interfaces.Notas;
using StudyFlow.Api.DTOs;
using StudyFlow.Api.Mappers;

namespace StudyFlow.Api.Services
{
    public class ConexaoNotaService : IConexaoNotaService
    {
        private readonly IConexaoNotaRepository _conexaoRepository;
        private readonly INotaRepository _notaRepository;

        public ConexaoNotaService(
            IConexaoNotaRepository conexaoRepository,
            INotaRepository notaRepository)
        {
            _conexaoRepository = conexaoRepository;
            _notaRepository = notaRepository;
        }

        public async Task<IEnumerable<ConexaoResponseDto>> ListarTodasAsync(int? temaId = null)
        {
            var conexoes = temaId.HasValue
                ? await _conexaoRepository.ObterPorTemaIdAsync(temaId.Value)
                : await _conexaoRepository.ObterTodasAsync();

            return conexoes.Select(c => c.toResponseDto());
        }

        public async Task<IEnumerable<ConexaoResponseDto>> ObterPorNotaIdAsync(int notaId)
        {
            var conexoes = await _conexaoRepository.ObterPorNotaIdAsync(notaId);
            return conexoes.Select(c => c.toResponseDto());
        }

        public async Task<ConexaoResponseDto?> ObterPorIdAsync(int id)
        {
            var conexao = await _conexaoRepository.ObterPorIdAsync(id);
            return conexao?.toResponseDto();
        }

        public async Task<ConexaoResponseDto?> CriarConexaoAsync(CreateConexaoDto dto)
        {
            ValidarConexaoParaSiMesma(dto.NotaOrigemId, dto.NotaDestinoId);

            if (!await AmbasNotasExistemAsync(dto.NotaOrigemId, dto.NotaDestinoId))
                return null;

            var conexaoExistente = await _conexaoRepository.ObterPorParAsync(dto.NotaOrigemId, dto.NotaDestinoId);
            if (conexaoExistente != null)
                return conexaoExistente.toResponseDto();

            var novaConexao = ConexaoNotaMapper.criarNovaConexao(dto.NotaOrigemId, dto.NotaDestinoId, dto.Rotulo);
            await _conexaoRepository.AdicionarAsync(novaConexao);
            await _conexaoRepository.SalvarAlteracoesAsync();

            return novaConexao.toResponseDto();
        }

        public async Task<bool> DeletarPorIdAsync(int id)
        {
            var conexao = await _conexaoRepository.ObterPorIdAsync(id);
            if (conexao == null) return false;

            _conexaoRepository.Remover(conexao);
            return await _conexaoRepository.SalvarAlteracoesAsync();
        }

        public async Task<bool> DeletarPorParAsync(int origemId, int destinoId)
        {
            var conexao = await _conexaoRepository.ObterPorParAsync(origemId, destinoId);
            if (conexao == null) return false;

            _conexaoRepository.Remover(conexao);
            return await _conexaoRepository.SalvarAlteracoesAsync();
        }


        
        private static void ValidarConexaoParaSiMesma(int origemId, int destinoId)
        {
            if (origemId == destinoId)
                throw new InvalidOperationException("Não é permitido conectar uma nota a ela mesma.");
        }

        private async Task<bool> AmbasNotasExistemAsync(int origemId, int destinoId)
        {
            var origem = await _notaRepository.ObterPorIdAsync(origemId);
            if (origem == null) return false;

            var destino = await _notaRepository.ObterPorIdAsync(destinoId);
            return destino != null;
        }

    }
}