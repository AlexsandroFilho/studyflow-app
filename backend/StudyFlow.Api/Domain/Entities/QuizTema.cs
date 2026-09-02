using StudyFlow.Api.Domain.Enums;

namespace StudyFlow.Api.Domain.Entities;

public class QuizTema
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int TemaId { get; set; }
    public Tema? Tema { get; set; }
    public Guid UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }
    public StatusQuizTema Status { get; set; }
    public string Mensagem { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public ICollection<QuizTemaPergunta> Perguntas { get; set; } = new List<QuizTemaPergunta>();
    public ICollection<TentativaQuizTema> Tentativas { get; set; } = new List<TentativaQuizTema>();
}
