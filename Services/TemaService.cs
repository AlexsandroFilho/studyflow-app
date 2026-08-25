using Microsoft.AspNetCore.Http.HttpResults;
using StudyFlow.Api.Domain.Entities;
using StudyFlow.Api.Domain.Interfaces.Temas;
using StudyFlow.Api.DTOs;
using StudyFlow.Api.Mappers;

namespace StudyFlow.Api.Services
{
    public class TemaService : ITemaService
    {

        private readonly ITemaRepository _temaRepository;

        public TemaService(ITemaRepository temaRepository)
        {
            _temaRepository = temaRepository;
        }

        public async Task<IEnumerable<TemaResponseDto>> ListarTodosAsync()
        {
            var temas = await _temaRepository.ListarTodosAsync();
            return temas.Select(t => t.toResponseDto());
        }

        public async Task<TemaResponseDto?> ObterPorIdAsync(int id)
        {
            var tema = await _temaRepository.ObterPorIdAsync(id);
            return tema?.toResponseDto();
        }

        public async Task<TemaResponseDto> CriarAsync(CreateTemaDto dto)
        {
            var tema = dto.toEntity();

            await _temaRepository.CriarAsync(tema);
            await _temaRepository.SalvarAlteracoesAsync();

            return tema.toResponseDto();
        }

        public async Task<bool> AtualizarAsync(int id, UpdateTemaDto dto)
        {
            var tema = await _temaRepository.ObterPorIdAsync(id);
            if (tema == null) return false;

            tema.Nome = dto.Nome.Trim();
            tema.Descricao = dto.Descricao?.Trim();

            _temaRepository.Atualizar(tema);
            return await _temaRepository.SalvarAlteracoesAsync();
        }

        public async Task<bool> DeletarAsync(int id)
        {
            var tema = await _temaRepository.ObterPorIdAsync(id);
            if (tema == null) return false;

            _temaRepository.Deletar(tema);
            return await _temaRepository.SalvarAlteracoesAsync();
        }
    }
}