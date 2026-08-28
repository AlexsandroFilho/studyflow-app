using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudyFlow.Api.Domain.Entities;

namespace StudyFlow.Api.Data.Configurations
{
    public class TemaConfiguration : IEntityTypeConfiguration<Tema>
    {
        public void Configure(EntityTypeBuilder<Tema> builder)
        {
            builder.ToTable("temas");
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Nome)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.Descricao)
                .HasMaxLength(255);

            builder.HasOne(t => t.Usuario)
                .WithMany(u => u.Temas)
                .HasForeignKey(t => t.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}