namespace StudyFlow.Api.Domain.Entities;

public class RespostaTentativaQuizTema
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TentativaQuizTemaId { get; set; }
    public TentativaQuizTema? TentativaQuizTema { get; set; }
    public Guid QuizTemaPerguntaId { get; set; }
    public QuizTemaPergunta? Pergunta { get; set; }
    public int IndiceAlternativaSelecionada { get; set; }
    public bool Acertou { get; set; }
}
