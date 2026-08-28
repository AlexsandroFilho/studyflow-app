
namespace StudyFlow.Api.Domain.Entities
{
    public class Usuario
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string SenhaHash { get; set; } = string.Empty;
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        public ICollection<Tema> Temas { get; set; } = new List<Tema>();
        public ICollection<Nota> Notas { get; set; } = new List<Nota>();
    }
}
