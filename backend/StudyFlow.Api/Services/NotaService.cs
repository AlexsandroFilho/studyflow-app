using StudyFlow.Api.Domain.Entities;
using StudyFlow.Api.Data;
using StudyFlow.Api.Domain.Interfaces.Notas;
using StudyFlow.Api.Domain.Interfaces.Temas;
using StudyFlow.Api.DTOs;
using StudyFlow.Api.Mappers;

namespace StudyFlow.Api.Services
{
    public class NotaService : INotaService
    {
        private readonly INotaRepository _notaRepository;
        private readonly ITemaRepository _temaRepository;
        private readonly AppDbContext _dbContext;

        public NotaService(
            INotaRepository notaRepository,
            ITemaRepository temaRepository,
            AppDbContext dbContext)
        {
            _notaRepository = notaRepository;
            _temaRepository = temaRepository;
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<NotaResponseDto>> ListarTodasAsync()
        {
            var notas = await _notaRepository.ListarTodasAsync();
            return notas.Select(n => n.toResponseDto());
        }

        public async Task<IEnumerable<NotaResponseDto>> ObterPorTemaIdAsync(int temaId)
        {
            var notas = await _notaRepository.ObterPorTemaIdAsync(temaId);
            return notas.Select(n => n.toResponseDto());
        }

        public async Task<NotaResponseDto?> ObterPorIdAsync(int id)
        {
            var nota = await _notaRepository.ObterPorIdAsync(id);
            return nota?.toResponseDto();
        }

        public async Task<NotaResponseDto?> CriarAsync(CreateNotaDto dto)
        {
            if (dto.TemaId.HasValue)
            {
                var temaExistente = await _temaRepository.ObterPorIdAsync(dto.TemaId.Value);
                if (temaExistente == null) return null;
            }

            var usuarioId = _dbContext.CurrentUsuarioId
                ?? throw new UnauthorizedAccessException("Usuário autenticado não encontrado.");
            var nota = dto.toEntity(usuarioId);

            await _notaRepository.CriarAsync(nota);
            await _notaRepository.SalvarAlteracoesAsync();

            if (dto.TemaId.HasValue)
            {
                nota.Tema = await _temaRepository.ObterPorIdAsync(dto.TemaId.Value);
            }

            return nota.toResponseDto();
        }

        public async Task<bool> AtualizarAsync(int id, UpdateNotaDto dto)
        {
            var nota = await _notaRepository.ObterPorIdAsync(id);
            if (nota == null) return false;

            if (dto.TemaId.HasValue)
            {
                var tema = await _temaRepository.ObterPorIdAsync(dto.TemaId.Value);
                if (tema == null) return false;

                nota.TemaId = dto.TemaId;
                nota.Tema = tema;
            }
            else
            {
                nota.TemaId = null;
                nota.Tema = null;
            }

            nota.Titulo = dto.Titulo;
            nota.Conteudo = dto.Conteudo;
            if (dto.ResumoIa != null) nota.ResumoIA = dto.ResumoIa;

            _notaRepository.Atualizar(nota);
            return await _notaRepository.SalvarAlteracoesAsync();
        }

        public async Task<bool> DeletarAsync(int id)
        {
            var nota = await _notaRepository.ObterPorIdAsync(id);
            if (nota == null) return false;

            _notaRepository.Deletar(nota);
            return await _notaRepository.SalvarAlteracoesAsync();
        }
    }
}