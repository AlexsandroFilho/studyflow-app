using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudyFlow.Api.Domain.Entities;

namespace StudyFlow.Api.Data.Configurations;

public sealed class ResumoTemaConfiguration : IEntityTypeConfiguration<ResumoTema>
{
    public void Configure(EntityTypeBuilder<ResumoTema> builder)
    {
        builder.ToTable("resumos_tema");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ResultadoJson).IsRequired();
        builder.Property(x => x.Modelo).HasMaxLength(100).IsRequired();
        builder.HasIndex(x => new { x.TemaId, x.DataCriacao });
        builder.HasOne(x => x.Tema).WithMany().HasForeignKey(x => x.TemaId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Usuario).WithMany().HasForeignKey(x => x.UsuarioId).OnDelete(DeleteBehavior.Restrict);
    }
}
