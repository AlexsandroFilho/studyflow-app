using System.Text.Json;
using StudyFlow.Api.Domain.Enums;
using StudyFlow.Api.Domain.Interfaces.Rag;
using StudyFlow.Api.DTOs;

namespace StudyFlow.Api.Services;

public sealed class GeminiRevisorAnatomia(IModeloIaClient modeloIaClient, IConfiguration configuration) : IRevisorAnatomia
{
    public string Modelo => configuration.GetSection("Ai").GetValue<string>("ChatModel") ?? "modelo-configurado";

    public async Task<ResultadoRevisaoNotaDto> RevisarAsync(ContextoNotaDto nota, IReadOnlyList<ContextoAnatomiaDto> evidencias, CancellationToken cancellationToken = default)
    {
        if (evidencias.Count == 0)
        {
            return new ResultadoRevisaoNotaDto(
                StatusRevisaoNota.EvidenciaInsuficiente,
                "Não foi possível validar esta nota porque o acervo oficial não retornou evidências suficientes.",
                [],
                [],
                []);
        }

        var respostaJson = await modeloIaClient.GerarJsonAsync(
            "Você é um revisor acadêmico de Anatomia. Responda apenas JSON válido.",
            MontarPrompt(nota, evidencias),
            cancellationToken);

        var modelo = JsonSerializer.Deserialize<RespostaModelo>(respostaJson, JsonOptions)
            ?? throw new InvalidOperationException("A IA não retornou uma revisão estruturada válida.");

        var referenciasPermitidas = evidencias.ToDictionary(x => x.ChunkId);
        var referencias = (modelo.Referencias ?? [])
            .Where(x => referenciasPermitidas.ContainsKey(x.ChunkId))
            .Select(x =>
            {
                var evidencia = referenciasPermitidas[x.ChunkId];
                return new ReferenciaAnatomiaDto(evidencia.FonteId, evidencia.Fonte, evidencia.Pagina, evidencia.Secao, evidencia.Assunto);
            })
            .Distinct()
            .ToList();

        var status = referencias.Count == 0 ? StatusRevisaoNota.EvidenciaInsuficiente : ConverterStatus(modelo.Status);
        return new ResultadoRevisaoNotaDto(
            status,
            modelo.Resumo?.Trim() ?? "Revisão concluída.",
            modelo.PontosCorretos?.Where(x => !string.IsNullOrWhiteSpace(x)).ToList() ?? [],
            modelo.Apontamentos?.Select(x => new ApontamentoRevisaoDto(x.Tipo ?? "observacao", x.Trecho ?? string.Empty, x.Explicacao ?? string.Empty, x.Sugestao)).ToList() ?? [],
            referencias);
    }

    private static string MontarPrompt(ContextoNotaDto nota, IReadOnlyList<ContextoAnatomiaDto> evidencias)
    {
        var conexoes = nota.Conexoes.Count == 0
            ? "Nenhuma nota diretamente conectada."
            : string.Join("\n", nota.Conexoes.Select(x => $"- Nota {x.NotaId}: {x.Titulo}\n{x.Conteudo}"));
        var fontes = string.Join("\n\n", evidencias.Select(x =>
            $"[CHUNK:{x.ChunkId}] Fonte: {x.Fonte}; página: {x.Pagina}; seção: {x.Secao ?? "não informada"}; assunto: {x.Assunto ?? "não informado"}\n{x.Texto}"));

        return $$"""
            Revise a anotação de Anatomia abaixo somente com base nas evidências oficiais fornecidas.
            Não trate as notas conectadas como fonte oficial. Não invente fatos nem referências.
            Se as evidências não bastarem, use status "evidenciaInsuficiente" e não faça correções factuais.

            NOTA ATUAL
            Título: {{nota.Titulo}}
            Conteúdo:
            {{nota.Conteudo}}

            NOTAS CONECTADAS DIRETAMENTE
            {{conexoes}}

            EVIDÊNCIAS OFICIAIS
            {{fontes}}

            Retorne apenas este JSON:
            {
              "status": "confirmada|possuiDivergencias|incompleta|evidenciaInsuficiente",
              "resumo": "texto curto",
              "pontosCorretos": ["..."],
              "apontamentos": [{"tipo":"divergencia|lacuna|observacao", "trecho":"...", "explicacao":"...", "sugestao":"..."}],
              "referencias": [{"chunkId":"GUID de um CHUNK acima"}]
            }
            """;
    }

    private static StatusRevisaoNota ConverterStatus(string? status) => status?.Trim().ToLowerInvariant() switch
    {
        "confirmada" => StatusRevisaoNota.Confirmada,
        "possuidivergencias" => StatusRevisaoNota.PossuiDivergencias,
        "incompleta" => StatusRevisaoNota.Incompleta,
        _ => StatusRevisaoNota.EvidenciaInsuficiente
    };

    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private sealed record RespostaModelo(string? Status, string? Resumo, List<string>? PontosCorretos, List<ApontamentoModelo>? Apontamentos, List<ReferenciaModelo>? Referencias);
    private sealed record ApontamentoModelo(string? Tipo, string? Trecho, string? Explicacao, string? Sugestao);
    private sealed record ReferenciaModelo(Guid ChunkId);
}
