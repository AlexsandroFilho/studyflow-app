using System.Text.Json;
using StudyFlow.Api.Domain.Enums;
using StudyFlow.Api.Domain.Interfaces.Rag;
using StudyFlow.Api.DTOs;

namespace StudyFlow.Api.Services;

public sealed class GeminiResumidorTemaAnatomia(IModeloIaClient modeloIaClient, IConfiguration configuration) : IResumidorTemaAnatomia
{
    public string Modelo => configuration.GetSection("Ai").GetValue<string>("ChatModel") ?? "modelo-configurado";

    public async Task<ResultadoResumoTemaDto> ResumirAsync(ContextoTemaDto tema, IReadOnlyList<ContextoAnatomiaDto> evidencias, CancellationToken cancellationToken = default)
    {
        if (evidencias.Count == 0)
            return EvidenciaInsuficiente("Não foi possível gerar um resumo fundamentado porque o acervo oficial não retornou evidências suficientes.");

        var respostaJson = await modeloIaClient.GerarJsonAsync(
            "Você é um assistente acadêmico de Anatomia. Responda apenas JSON válido.",
            MontarPrompt(tema, evidencias),
            cancellationToken);

        var modelo = JsonSerializer.Deserialize<RespostaModelo>(respostaJson, JsonOptions)
            ?? throw new InvalidOperationException("A IA não retornou um resumo estruturado válido.");

        var evidenciasPorId = evidencias.Select((evidencia, indice) => new { Id = indice + 1, Evidencia = evidencia })
            .ToDictionary(x => x.Id, x => x.Evidencia);
        var referencias = (modelo.Referencias ?? [])
            .Where(x => evidenciasPorId.ContainsKey(x.EvidenciaId))
            .Select(x =>
            {
                var evidencia = evidenciasPorId[x.EvidenciaId];
                return new ReferenciaAnatomiaDto(evidencia.FonteId, evidencia.Fonte, evidencia.Pagina, evidencia.Secao, evidencia.Assunto);
            })
            .Distinct()
            .ToList();

        if (!string.Equals(modelo.Status, "gerado", StringComparison.OrdinalIgnoreCase) || referencias.Count == 0)
            return EvidenciaInsuficiente("Não foi possível gerar um resumo fundamentado porque a IA não indicou referências válidas do acervo oficial.");

        var conexoesPorId = tema.Conexoes.ToDictionary(x => x.ConexaoId);
        var relacoes = (modelo.Relacoes ?? [])
            .Where(x => conexoesPorId.ContainsKey(x.ConexaoId) && !string.IsNullOrWhiteSpace(x.Descricao))
            .Select(x =>
            {
                var conexao = conexoesPorId[x.ConexaoId];
                return new RelacaoResumoTemaDto(
                    conexao.ConexaoId,
                    conexao.NotaOrigemId,
                    conexao.TituloOrigem,
                    conexao.NotaDestinoId,
                    conexao.TituloDestino,
                    conexao.Rotulo,
                    x.Descricao!.Trim());
            })
            .ToList();

        return new ResultadoResumoTemaDto(
            StatusResumoTema.Gerado,
            modelo.Resumo?.Trim() ?? "Resumo gerado com base no acervo oficial.",
            modelo.PontosChave?.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).ToList() ?? [],
            relacoes,
            referencias);
    }

    private static ResultadoResumoTemaDto EvidenciaInsuficiente(string mensagem) => new(
        StatusResumoTema.EvidenciaInsuficiente, mensagem, [], [], []);

    private static string MontarPrompt(ContextoTemaDto tema, IReadOnlyList<ContextoAnatomiaDto> evidencias)
    {
        var notas = string.Join("\n\n", tema.Notas.Select(x => $"[NOTA:{x.NotaId}] {x.Titulo}\n{x.Conteudo}"));
        var conexoes = tema.Conexoes.Count == 0
            ? "Nenhuma conexão visual interna foi criada neste tema."
            : string.Join("\n", tema.Conexoes.Select(x => $"[CONEXAO:{x.ConexaoId}] {x.TituloOrigem} -> {x.TituloDestino}; rótulo: {x.Rotulo ?? "não informado"}"));
        var fontes = string.Join("\n\n", evidencias.Select((x, indice) =>
            $"[EVIDENCIA:{indice + 1}] Fonte: {x.Fonte}; página: {x.Pagina}; seção: {x.Secao ?? "não informada"}; assunto: {x.Assunto ?? "não informado"}\n{x.Texto}"));

        return $$"""
            Gere um resumo de Anatomia do tema abaixo somente com base nas evidências oficiais fornecidas.
            As notas do aluno e as conexões visuais são contexto para organizar o resumo, mas não são fontes oficiais.
            Não invente fatos, relações ou referências. Se a evidência não for suficiente, use "evidenciaInsuficiente".
            Em "referencias", use exclusivamente os números de EVIDENCIA apresentados acima.
            Em "relacoes", use exclusivamente os números de CONEXAO apresentados acima e apenas quando a relação puder ser explicada sem inventar fatos.

            TEMA
            Nome: {{tema.Nome}}
            Descrição: {{tema.Descricao ?? "não informada"}}

            NOTAS DO TEMA
            {{notas}}

            CONEXÕES VISUAIS INTERNAS DO TEMA
            {{conexoes}}

            EVIDÊNCIAS OFICIAIS
            {{fontes}}

            Retorne apenas este JSON:
            {
              "status": "gerado|evidenciaInsuficiente",
              "resumo": "texto do resumo",
              "pontosChave": ["..."],
              "relacoes": [{"conexaoId": 1, "descricao": "como os dois conceitos se relacionam"}],
              "referencias": [{"evidenciaId": 1}]
            }
            """;
    }

    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private sealed record RespostaModelo(string? Status, string? Resumo, List<string>? PontosChave, List<RelacaoModelo>? Relacoes, List<ReferenciaModelo>? Referencias);
    private sealed record RelacaoModelo(int ConexaoId, string? Descricao);
    private sealed record ReferenciaModelo(int EvidenciaId);
}
