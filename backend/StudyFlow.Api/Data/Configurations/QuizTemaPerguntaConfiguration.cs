using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudyFlow.Api.Domain.Entities;

namespace StudyFlow.Api.Data.Configurations;

public sealed class QuizTemaPerguntaConfiguration : IEntityTypeConfiguration<QuizTemaPergunta>
{
    public void Configure(EntityTypeBuilder<QuizTemaPergunta> builder)
    {
        builder.ToTable("quiz_tema_perguntas");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Enunciado).HasMaxLength(1000).IsRequired();
        builder.Property(x => x.AlternativasJson).IsRequired();
        builder.Property(x => x.Explicacao).HasMaxLength(2000).IsRequired();
        builder.Property(x => x.ReferenciasJson).IsRequired();
        builder.HasIndex(x => new { x.QuizTemaId, x.Ordem }).IsUnique();
        builder.HasOne(x => x.QuizTema).WithMany(x => x.Perguntas).HasForeignKey(x => x.QuizTemaId).OnDelete(DeleteBehavior.Cascade);
    }
}
