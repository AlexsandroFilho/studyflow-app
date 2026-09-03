using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudyFlow.Api.Domain.Entities;

namespace StudyFlow.Api.Data.Configurations;

public sealed class IngestaoFonteAnatomiaConfiguration : IEntityTypeConfiguration<IngestaoFonteAnatomia>
{
    public void Configure(EntityTypeBuilder<IngestaoFonteAnatomia> builder)
    {
        builder.ToTable("ingestoes_fontes_anatomia");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Titulo).HasMaxLength(300).IsRequired();
        builder.Property(x => x.Autor).HasMaxLength(300);
        builder.Property(x => x.Versao).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Assunto).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Subassunto).HasMaxLength(200);
        builder.Property(x => x.ArquivoTemporarioChave).HasMaxLength(1000).IsRequired();
        builder.Property(x => x.MensagemErro).HasMaxLength(2000);
        builder.HasIndex(x => new { x.Status, x.DataCriacao });
        builder.HasIndex(x => x.UsuarioId);
        builder.HasOne(x => x.Usuario).WithMany().HasForeignKey(x => x.UsuarioId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.FonteAnatomia).WithMany().HasForeignKey(x => x.FonteAnatomiaId).OnDelete(DeleteBehavior.SetNull);
    }
}
