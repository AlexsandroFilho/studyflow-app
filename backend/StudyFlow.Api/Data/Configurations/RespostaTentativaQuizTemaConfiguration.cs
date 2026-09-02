using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudyFlow.Api.Domain.Entities;

namespace StudyFlow.Api.Data.Configurations;

public sealed class RespostaTentativaQuizTemaConfiguration : IEntityTypeConfiguration<RespostaTentativaQuizTema>
{
    public void Configure(EntityTypeBuilder<RespostaTentativaQuizTema> builder)
    {
        builder.ToTable("respostas_tentativa_quiz_tema");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.TentativaQuizTemaId, x.QuizTemaPerguntaId }).IsUnique();
        builder.HasOne(x => x.TentativaQuizTema).WithMany(x => x.Respostas).HasForeignKey(x => x.TentativaQuizTemaId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Pergunta).WithMany().HasForeignKey(x => x.QuizTemaPerguntaId).OnDelete(DeleteBehavior.Cascade);
    }
}
