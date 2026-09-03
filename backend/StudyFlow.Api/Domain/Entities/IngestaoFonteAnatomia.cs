using StudyFlow.Api.Domain.Enums;

namespace StudyFlow.Api.Domain.Entities;

public sealed class IngestaoFonteAnatomia
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }
    public Guid? FonteAnatomiaId { get; set; }
    public FonteAnatomia? FonteAnatomia { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Autor { get; set; }
    public string Versao { get; set; } = string.Empty;
    public string Assunto { get; set; } = string.Empty;
    public string? Subassunto { get; set; }
    public string ArquivoTemporarioChave { get; set; } = string.Empty;
    public StatusIngestaoFonteAnatomia Status { get; set; } = StatusIngestaoFonteAnatomia.Pendente;
    public string? MensagemErro { get; set; }
    public int QuantidadeChunks { get; set; }
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public DateTime? DataInicio { get; set; }
    public DateTime? DataConclusao { get; set; }

    public void Iniciar()
    {
        Status = StatusIngestaoFonteAnatomia.Processando;
        DataInicio = DateTime.UtcNow;
        MensagemErro = null;
    }

    public void Concluir(Guid fonteId, int quantidadeChunks)
    {
        Status = StatusIngestaoFonteAnatomia.Concluida;
        FonteAnatomiaId = fonteId;
        QuantidadeChunks = quantidadeChunks;
        DataConclusao = DateTime.UtcNow;
        MensagemErro = null;
    }

    public void Falhar(string mensagem)
    {
        Status = StatusIngestaoFonteAnatomia.Falhou;
        DataConclusao = DateTime.UtcNow;
        MensagemErro = mensagem;
    }

    public void Reenfileirar()
    {
        Status = StatusIngestaoFonteAnatomia.Pendente;
        DataInicio = null;
        DataConclusao = null;
        MensagemErro = null;
    }
}
