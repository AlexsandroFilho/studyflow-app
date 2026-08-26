using StudyFlow.Api.Domain.Entities;
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

        public NotaService(INotaRepository notaRepository, ITemaRepository temaRepository)
        {
            _notaRepository = notaRepository;
            _temaRepository = temaRepository;
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
            var temaExistente = await _temaRepository.ObterPorIdAsync(dto.TemaId);
            if (temaExistente == null) return null;

            var nota = dto.toEntity();

            await _notaRepository.CriarAsync(nota);
            await _notaRepository.SalvarAlteracoesAsync();

            nota.Tema = temaExistente;

            return nota.toResponseDto();
        }

        public async Task<bool> AtualizarAsync(int id, UpdateNotaDto dto)
        {
            var nota = await _notaRepository.ObterPorIdAsync(id);
            if (nota == null) return false;

            var tema = await _temaRepository.ObterPorIdAsync(dto.TemaId);
            if (tema == null) return false;

            nota.Titulo = dto.Titulo;
            nota.Conteudo = dto.Conteudo;
            nota.TemaId = dto.TemaId;
            nota.Tema = tema;
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