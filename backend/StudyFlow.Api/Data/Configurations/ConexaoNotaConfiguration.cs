using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudyFlow.Api.Domain.Entities;

namespace StudyFlow.Api.Data.Configurations
{
    public class ConexaoNotaConfiguration : IEntityTypeConfiguration<ConexaoNota>
    {
        public void Configure(EntityTypeBuilder<ConexaoNota> builder)
        {
            builder.ToTable("nota_conexoes");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Rotulo)
                .HasMaxLength(100);

            builder.HasIndex(c => new { c.NotaOrigemId, c.NotaDestinoId })
                .IsUnique()
                .HasDatabaseName("uq_conexao_origem_destino");

            builder.HasOne(c => c.NotaOrigem)
                .WithMany(n => n.ConexoesOrigem)
                .HasForeignKey(c => c.NotaOrigemId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.NotaDestino)
                .WithMany(n => n.ConexoesDestino)
                .HasForeignKey(c => c.NotaDestinoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}