using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudyFlow.Api.Domain.Entities;

namespace StudyFlow.Api.Data.Configurations;

public sealed class QuizTemaConfiguration : IEntityTypeConfiguration<QuizTema>
{
    public void Configure(EntityTypeBuilder<QuizTema> builder)
    {
        builder.ToTable("quizzes_tema");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Mensagem).HasMaxLength(500).IsRequired();
        builder.Property(x => x.Modelo).HasMaxLength(100).IsRequired();
        builder.HasIndex(x => new { x.TemaId, x.DataCriacao });
        builder.HasIndex(x => x.UsuarioId);
        builder.HasOne(x => x.Tema).WithMany().HasForeignKey(x => x.TemaId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Usuario).WithMany().HasForeignKey(x => x.UsuarioId).OnDelete(DeleteBehavior.Restrict);
    }
}
