using System.Text.Json;
using StudyFlow.Api.Domain.Entities;
using StudyFlow.Api.DTOs;

namespace StudyFlow.Api.Mappers;

public static class QuizTemaMapper
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public static QuizTema ToEntity(this ResultadoGeracaoQuizTemaDto resultado, int temaId, Guid usuarioId, string modelo)
    {
        var quiz = new QuizTema
        {
            TemaId = temaId,
            UsuarioId = usuarioId,
            Status = resultado.Status,
            Mensagem = resultado.Mensagem,
            Modelo = modelo
        };

        quiz.Perguntas = resultado.Perguntas.Select((pergunta, indice) => new QuizTemaPergunta
        {
            QuizTemaId = quiz.Id,
            Ordem = indice + 1,
            Enunciado = pergunta.Enunciado,
            AlternativasJson = JsonSerializer.Serialize(pergunta.Alternativas),
            IndiceRespostaCorreta = pergunta.IndiceRespostaCorreta,
            Explicacao = pergunta.Explicacao,
            ReferenciasJson = JsonSerializer.Serialize(pergunta.Referencias)
        }).ToList();
        return quiz;
    }

    public static QuizTemaResponseDto ToPublicResponseDto(this QuizTema quiz) => new(
        quiz.Id,
        quiz.TemaId,
        quiz.Status,
        quiz.Mensagem,
        quiz.Perguntas.OrderBy(x => x.Ordem).Select(x => new PerguntaQuizResponseDto(
            x.Id,
            x.Ordem,
            x.Enunciado,
            DesserializarAlternativas(x.AlternativasJson))).ToList(),
        quiz.Modelo,
        quiz.DataCriacao);

    public static TentativaQuizTemaResponseDto ToResponseDto(this TentativaQuizTema tentativa)
    {
        var correcoes = tentativa.Respostas
            .Where(x => x.Pergunta is not null)
            .OrderBy(x => x.Pergunta!.Ordem)
            .Select(x => new CorrecaoPerguntaQuizDto(
                x.QuizTemaPerguntaId,
                x.Pergunta!.Ordem,
                x.Pergunta.Enunciado,
                DesserializarAlternativas(x.Pergunta.AlternativasJson),
                x.IndiceAlternativaSelecionada,
                x.Pergunta.IndiceRespostaCorreta,
                x.Acertou,
                x.Pergunta.Explicacao,
                DesserializarReferencias(x.Pergunta.ReferenciasJson)))
            .ToList();

        var percentual = tentativa.QuantidadeQuestoes == 0
            ? 0
            : Math.Round(tentativa.QuantidadeAcertos * 100d / tentativa.QuantidadeQuestoes, 2);
        return new TentativaQuizTemaResponseDto(
            tentativa.Id,
            tentativa.QuizTemaId,
            tentativa.QuantidadeAcertos,
            tentativa.QuantidadeQuestoes,
            percentual,
            correcoes,
            tentativa.DataCriacao);
    }

    public static IReadOnlyList<string> DesserializarAlternativas(string json) =>
        JsonSerializer.Deserialize<List<string>>(json, JsonOptions) ?? [];

    public static IReadOnlyList<ReferenciaAnatomiaDto> DesserializarReferencias(string json) =>
        JsonSerializer.Deserialize<List<ReferenciaAnatomiaDto>>(json, JsonOptions) ?? [];
}
