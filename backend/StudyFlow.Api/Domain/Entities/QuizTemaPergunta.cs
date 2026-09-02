namespace StudyFlow.Api.Domain.Entities;

public class QuizTemaPergunta
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid QuizTemaId { get; set; }
    public QuizTema? QuizTema { get; set; }
    public int Ordem { get; set; }
    public string Enunciado { get; set; } = string.Empty;
    public string AlternativasJson { get; set; } = string.Empty;
    public int IndiceRespostaCorreta { get; set; }
    public string Explicacao { get; set; } = string.Empty;
    public string ReferenciasJson { get; set; } = string.Empty;
}
