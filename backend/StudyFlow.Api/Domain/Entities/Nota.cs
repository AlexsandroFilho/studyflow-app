
namespace StudyFlow.Api.Domain.Entities
{
    public class Nota
    {
        public int Id { get; set; }
        public string? Titulo { get; set; }
        public string? Conteudo { get; set; }
        public string? ResumoIA { get; set; }
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        public Guid UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        public int? TemaId { get; set; }
        public Tema? Tema { get; set; }

        public ICollection<ConexaoNota> ConexoesOrigem { get; set; } = new List<ConexaoNota>();
        public ICollection<ConexaoNota> ConexoesDestino { get; set; } = new List<ConexaoNota>();
    }
}