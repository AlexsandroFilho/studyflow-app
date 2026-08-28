using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudyFlow.Api.Domain.Entities;

namespace StudyFlow.Api.Data.Configurations
{
    public class NotaConfiguration : IEntityTypeConfiguration<Nota>
    {
        public void Configure(EntityTypeBuilder<Nota> builder)
        {
            builder.ToTable("notas");
            builder.HasKey(n => n.Id);

            builder.Property(n => n.Titulo)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(n => n.Conteudo)
                .IsRequired();

            builder.Property(n => n.TemaId)
                .IsRequired(false);

            builder.HasOne(n => n.Usuario)
                .WithMany(u => u.Notas)
                .HasForeignKey(n => n.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(n => n.Tema)
                .WithMany(t => t.Notas)
                .HasForeignKey(n => n.TemaId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}