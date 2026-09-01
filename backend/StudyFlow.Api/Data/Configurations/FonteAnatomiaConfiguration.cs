using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudyFlow.Api.Domain.Entities;

namespace StudyFlow.Api.Data.Configurations;

public class FonteAnatomiaConfiguration : IEntityTypeConfiguration<FonteAnatomia>
{
    public void Configure(EntityTypeBuilder<FonteAnatomia> builder)
    {
        builder.ToTable("fontes_anatomia");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Titulo).HasMaxLength(300).IsRequired();
        builder.Property(x => x.Autor).HasMaxLength(300);
        builder.Property(x => x.Versao).HasMaxLength(100).IsRequired();
        builder.Property(x => x.ArquivoChave).HasMaxLength(1000).IsRequired();
        builder.Property(x => x.HashConteudo).HasMaxLength(64).IsRequired();
        builder.HasIndex(x => x.HashConteudo).IsUnique();
    }
}
