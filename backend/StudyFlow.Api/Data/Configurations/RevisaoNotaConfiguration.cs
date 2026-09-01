using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudyFlow.Api.Domain.Entities;

namespace StudyFlow.Api.Data.Configurations;

public class RevisaoNotaConfiguration : IEntityTypeConfiguration<RevisaoNota>
{
    public void Configure(EntityTypeBuilder<RevisaoNota> builder)
    {
        builder.ToTable("revisoes_nota");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ResultadoJson).IsRequired();
        builder.Property(x => x.Modelo).HasMaxLength(100).IsRequired();
        builder.HasIndex(x => new { x.NotaId, x.DataCriacao });
        builder.HasOne(x => x.Nota).WithMany().HasForeignKey(x => x.NotaId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Usuario).WithMany().HasForeignKey(x => x.UsuarioId).OnDelete(DeleteBehavior.Restrict);
    }
}
