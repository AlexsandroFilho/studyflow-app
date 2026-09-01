using Pgvector;
using StudyFlow.Api.Domain.Entities;
using StudyFlow.Api.DTOs;

namespace StudyFlow.Api.Mappers;

public static class AnatomiaChunkMapper
{
    public static ContextoAnatomiaDto ToContextoDto(this AnatomiaChunkVector chunk, Pgvector.Vector consulta) => new(
        chunk.Id,
        chunk.FonteAnatomiaId,
        chunk.FonteAnatomia?.Titulo ?? "Fonte sem título",
        chunk.Pagina,
        chunk.Secao,
        chunk.Assunto,
        chunk.Texto,
        CalcularSimilaridadeCoseno(chunk.Embedding, consulta));

    private static double CalcularSimilaridadeCoseno(Vector primeiro, Vector segundo)
    {
        var valoresPrimeiro = primeiro.ToArray();
        var valoresSegundo = segundo.ToArray();

        if (valoresPrimeiro.Length != valoresSegundo.Length)
        {
            return 0;
        }

        double produtoEscalar = 0;
        double normaPrimeiro = 0;
        double normaSegundo = 0;

        for (var indice = 0; indice < valoresPrimeiro.Length; indice++)
        {
            produtoEscalar += valoresPrimeiro[indice] * valoresSegundo[indice];
            normaPrimeiro += valoresPrimeiro[indice] * valoresPrimeiro[indice];
            normaSegundo += valoresSegundo[indice] * valoresSegundo[indice];
        }

        if (normaPrimeiro == 0 || normaSegundo == 0)
        {
            return 0;
        }

        return Math.Clamp(
            produtoEscalar / (Math.Sqrt(normaPrimeiro) * Math.Sqrt(normaSegundo)),
            -1,
            1);
    }
}
