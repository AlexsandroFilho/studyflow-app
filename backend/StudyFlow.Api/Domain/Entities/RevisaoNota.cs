using StudyFlow.Api.Domain.Enums;

namespace StudyFlow.Api.Domain.Entities;

public class RevisaoNota
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int NotaId { get; set; }
    public Nota? Nota { get; set; }
    public Guid UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }
    public StatusRevisaoNota Status { get; set; }
    public string ResultadoJson { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
}
