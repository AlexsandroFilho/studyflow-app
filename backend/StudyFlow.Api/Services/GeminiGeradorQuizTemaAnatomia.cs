using System.Text.Json;
using StudyFlow.Api.Domain.Enums;
using StudyFlow.Api.Domain.Interfaces.Rag;
using StudyFlow.Api.DTOs;

namespace StudyFlow.Api.Services;

public sealed class GeminiGeradorQuizTemaAnatomia(IModeloIaClient modeloIaClient, IConfiguration configuration) : IGeradorQuizTemaAnatomia
{
    private const int QuantidadePerguntas = 5;
    private const int QuantidadeAlternativas = 4;

    public string Modelo => configuration.GetSection("Ai").GetValue<string>("ChatModel") ?? "modelo-configurado";

    public async Task<ResultadoGeracaoQuizTemaDto> GerarAsync(ContextoTemaDto tema, IReadOnlyList<ContextoAnatomiaDto> evidencias, CancellationToken cancellationToken = default)
    {
        if (evidencias.Count == 0)
            return EvidenciaInsuficiente("Não foi possível gerar o quiz porque o acervo oficial não retornou evidências suficientes.");

        var respostaJson = await modeloIaClient.GerarJsonAsync(
            "Você cria avaliações acadêmicas de Anatomia. Responda apenas JSON válido.",
            MontarPrompt(tema, evidencias),
            cancellationToken);
        var modelo = JsonSerializer.Deserialize<RespostaModelo>(respostaJson, JsonOptions);

        if (modelo?.Perguntas is null || modelo.Perguntas.Count != QuantidadePerguntas)
            return EvidenciaInsuficiente("A IA não retornou cinco perguntas válidas e fundamentadas.");

        var evidenciasPorId = evidencias.Select((evidencia, indice) => new { Id = indice + 1, Evidencia = evidencia })
            .ToDictionary(x => x.Id, x => x.Evidencia);
        var perguntas = new List<PerguntaQuizGeradaDto>();

        foreach (var pergunta in modelo.Perguntas)
        {
            if (!PerguntaEstruturalmenteValida(pergunta))
                return EvidenciaInsuficiente("A IA retornou uma pergunta fora do formato esperado.");

            var referencias = (pergunta.EvidenciaIds ?? [])
                .Distinct()
                .Where(evidenciasPorId.ContainsKey)
                .Select(id =>
                {
                    var evidencia = evidenciasPorId[id];
                    return new ReferenciaAnatomiaDto(evidencia.FonteId, evidencia.Fonte, evidencia.Pagina, evidencia.Secao, evidencia.Assunto);
                })
                .Distinct()
                .ToList();
            if (referencias.Count == 0)
                return EvidenciaInsuficiente("A IA retornou uma pergunta sem referência oficial válida.");

            perguntas.Add(new PerguntaQuizGeradaDto(
                pergunta.Enunciado!.Trim(),
                pergunta.Alternativas!.Select(x => x.Trim()).ToList(),
                pergunta.IndiceRespostaCorreta,
                pergunta.Explicacao!.Trim(),
                referencias));
        }

        return new ResultadoGeracaoQuizTemaDto(StatusQuizTema.Gerado, "Quiz gerado com base no acervo oficial.", perguntas);
    }

    private static bool PerguntaEstruturalmenteValida(PerguntaModelo pergunta) =>
        !string.IsNullOrWhiteSpace(pergunta.Enunciado)
        && !string.IsNullOrWhiteSpace(pergunta.Explicacao)
        && pergunta.Alternativas is { Count: QuantidadeAlternativas }
        && pergunta.Alternativas.All(x => !string.IsNullOrWhiteSpace(x))
        && pergunta.Alternativas.Distinct(StringComparer.OrdinalIgnoreCase).Count() == QuantidadeAlternativas
        && pergunta.IndiceRespostaCorreta is >= 0 and < QuantidadeAlternativas;

    private static ResultadoGeracaoQuizTemaDto EvidenciaInsuficiente(string mensagem) =>
        new(StatusQuizTema.EvidenciaInsuficiente, mensagem, []);

    private static string MontarPrompt(ContextoTemaDto tema, IReadOnlyList<ContextoAnatomiaDto> evidencias)
    {
        var notas = string.Join("\n\n", tema.Notas.Select(x => $"[NOTA:{x.NotaId}] {x.Titulo}\n{x.Conteudo}"));
        var conexoes = tema.Conexoes.Count == 0
            ? "Nenhuma conexão interna."
            : string.Join("\n", tema.Conexoes.Select(x => $"[CONEXAO:{x.ConexaoId}] {x.TituloOrigem} -> {x.TituloDestino}; rótulo: {x.Rotulo ?? "não informado"}"));
        var fontes = string.Join("\n\n", evidencias.Select((x, indice) =>
            $"[EVIDENCIA:{indice + 1}] Fonte: {x.Fonte}; página: {x.Pagina}; seção: {x.Secao ?? "não informada"}; assunto: {x.Assunto ?? "não informado"}\n{x.Texto}"));

        return $$"""
            Crie exatamente cinco questões intermediárias de múltipla escolha sobre o tema de Anatomia abaixo.
            Cada questão deve ter exatamente quatro alternativas distintas e somente uma correta.
            Use notas e conexões apenas para escolher os assuntos. Todo fato, gabarito e explicação deve estar fundamentado nas evidências oficiais.
            Não invente conteúdo. Use somente números de EVIDENCIA fornecidos.

            TEMA: {{tema.Nome}}
            DESCRIÇÃO: {{tema.Descricao ?? "não informada"}}

            NOTAS DO ALUNO
            {{notas}}

            CONEXÕES INTERNAS
            {{conexoes}}

            EVIDÊNCIAS OFICIAIS
            {{fontes}}

            Retorne apenas este JSON:
            {
              "perguntas": [{
                "enunciado": "...",
                "alternativas": ["...", "...", "...", "..."],
                "indiceRespostaCorreta": 0,
                "explicacao": "...",
                "evidenciaIds": [1]
              }]
            }
            O índice correto deve ser de 0 a 3. Distribua as respostas corretas entre posições diferentes.
            """;
    }

    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private sealed record RespostaModelo(List<PerguntaModelo>? Perguntas);
    private sealed record PerguntaModelo(string? Enunciado, List<string>? Alternativas, int IndiceRespostaCorreta, string? Explicacao, List<int>? EvidenciaIds);
}
