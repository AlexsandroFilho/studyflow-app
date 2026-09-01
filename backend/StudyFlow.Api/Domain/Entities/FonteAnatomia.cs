namespace StudyFlow.Api.Domain.Entities;

public class FonteAnatomia
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Titulo { get; set; } = string.Empty;
    public string? Autor { get; set; }
    public string Versao { get; set; } = string.Empty;
    public string ArquivoChave { get; set; } = string.Empty;
    public string HashConteudo { get; set; } = string.Empty;
    public bool Publicada { get; set; }
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    public ICollection<AnatomiaChunkVector> Chunks { get; set; } = new List<AnatomiaChunkVector>();
}
