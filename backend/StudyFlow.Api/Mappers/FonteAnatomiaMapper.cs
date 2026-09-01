using Pgvector;
using StudyFlow.Api.Domain.Entities;
using StudyFlow.Api.DTOs;

namespace StudyFlow.Api.Mappers;

public static class FonteAnatomiaMapper
{
    public static FonteAnatomia ToEntity(this FonteIngestaoRequest request, string arquivoChave, string hashConteudo) => new()
    {
        Titulo = request.Titulo.Trim(),
        Autor = request.Autor?.Trim(),
        Versao = request.Versao.Trim(),
        ArquivoChave = arquivoChave,
        HashConteudo = hashConteudo,
        Publicada = false
    };

    public static void Atualizar(this FonteAnatomia fonte, FonteIngestaoRequest request, string arquivoChave)
    {
        fonte.Titulo = request.Titulo.Trim();
        fonte.Autor = request.Autor?.Trim();
        fonte.Versao = request.Versao.Trim();
        fonte.ArquivoChave = arquivoChave;
    }

    public static AnatomiaChunkVector ToChunkEntity(this ChunkAnatomiaDto chunk, FonteIngestaoRequest request, float[] embedding) => new()
    {
        Texto = chunk.Texto,
        Pagina = chunk.Pagina,
        Secao = chunk.Secao,
        Assunto = request.Assunto?.Trim(),
        Subassunto = request.Subassunto?.Trim(),
        Embedding = new Vector(embedding)
    };
}
