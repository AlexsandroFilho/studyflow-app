
using StudyFlow.Api.Domain.Enums;

namespace StudyFlow.Api.Domain.Entities
{
    public class Usuario
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string SenhaHash { get; set; } = string.Empty;
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
        public UserRole Role { get; set; } = UserRole.User;
        public bool MostrarGuiaInicial { get; set; } = true;

        public ICollection<Tema> Temas { get; set; } = new List<Tema>();
        public ICollection<Nota> Notas { get; set; } = new List<Nota>();
    }
}
