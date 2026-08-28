
namespace StudyFlow.Api.Domain.Entities
{
    public class Tema
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public string? Descricao { get; set; }
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        public Guid UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        public ICollection<Nota> Notas { get; set; } = new List<Nota>();
    }
}