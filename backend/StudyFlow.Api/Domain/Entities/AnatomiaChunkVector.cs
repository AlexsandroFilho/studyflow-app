using Pgvector;

namespace StudyFlow.Api.Domain.Entities;

public class AnatomiaChunkVector
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid FonteAnatomiaId { get; set; }
    public FonteAnatomia? FonteAnatomia { get; set; }
    public string Texto { get; set; } = string.Empty;
    public int Pagina { get; set; }
    public string? Secao { get; set; }
    public string? Assunto { get; set; }
    public string? Subassunto { get; set; }
    public Vector Embedding { get; set; } = new(Array.Empty<float>());
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
}
