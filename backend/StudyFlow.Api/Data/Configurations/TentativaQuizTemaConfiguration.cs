using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudyFlow.Api.Domain.Entities;

namespace StudyFlow.Api.Data.Configurations;

public sealed class TentativaQuizTemaConfiguration : IEntityTypeConfiguration<TentativaQuizTema>
{
    public void Configure(EntityTypeBuilder<TentativaQuizTema> builder)
    {
        builder.ToTable("tentativas_quiz_tema");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.QuizTemaId, x.DataCriacao });
        builder.HasIndex(x => x.UsuarioId);
        builder.HasOne(x => x.QuizTema).WithMany(x => x.Tentativas).HasForeignKey(x => x.QuizTemaId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Usuario).WithMany().HasForeignKey(x => x.UsuarioId).OnDelete(DeleteBehavior.Restrict);
    }
}
