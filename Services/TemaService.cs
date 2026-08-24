using Microsoft.AspNetCore.Http.HttpResults;
using StudyFlow.Api.Domain.Entities;
using StudyFlow.Api.Domain.Interfaces.Temas;
using StudyFlow.Api.DTOs;

namespace StudyFlow.Api.Services
{
    public class TemaService : ITemaService
    {

        private readonly ITemaRepository _temaRepository;

        public TemaService(ITemaRepository temaRepository)
        {
            _temaRepository = temaRepository;
        }

        public async Task<bool> AtualizarAsync(int id, UpdateTemaDto dto)
        {
            var tema = await _temaRepository.ObterPorIdAsync(id);
            if(tema == null) return false;
           
           tema.Nome = dto.Nome;
           tema.Descricao = dto.Descricao;

            _temaRepository.Atualizar(tema);

            return await _temaRepository.SalvarAlteracoesAsync();
        }

        public async Task<TemaResponseDto> CriarAsync(CreateTemaDto dto)
        {
            var tema = new Tema
            {
                Nome = dto.Nome,
                Descricao = dto.Descricao,
                DataCriacao = DateTime.UtcNow
            };

            await _temaRepository.CriarAsync(tema);
            await _temaRepository.SalvarAlteracoesAsync();

            return new TemaResponseDto(tema.Id, tema.Nome, tema.Descricao, tema.DataCriacao);

        }

        public async Task<bool> DeletarAsync(int id)
        {
            var tema = await _temaRepository.ObterPorIdAsync(id);
            if(tema == null) return false;

            _temaRepository.Deletar(tema);
            return await _temaRepository.SalvarAlteracoesAsync();

        }

        public async Task<IEnumerable<TemaResponseDto>> ListarTodosAsync()
        {
            var temas = await _temaRepository.ListarTodosAsync();

            return temas.Select(t => new TemaResponseDto(
                t.Id,
                t.Nome!,
                t.Descricao,
                t.DataCriacao
            ));
        }

        public async Task<TemaResponseDto?> ObterPorIdAsync(int id)
        {
            var tema = await _temaRepository.ObterPorIdAsync(id);
            if(tema == null) return null;

            return new TemaResponseDto(tema.Id, tema.Nome!, tema.Descricao, tema.DataCriacao);
        }
    }
}