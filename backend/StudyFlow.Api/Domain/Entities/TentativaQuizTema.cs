namespace StudyFlow.Api.Domain.Entities;

public class TentativaQuizTema
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid QuizTemaId { get; set; }
    public QuizTema? QuizTema { get; set; }
    public Guid UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }
    public int QuantidadeAcertos { get; set; }
    public int QuantidadeQuestoes { get; set; }
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public ICollection<RespostaTentativaQuizTema> Respostas { get; set; } = new List<RespostaTentativaQuizTema>();
}
