using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudyFlow.Api.Domain.Entities;

namespace StudyFlow.Api.Data.Configurations;

public class AnatomiaChunkVectorConfiguration : IEntityTypeConfiguration<AnatomiaChunkVector>
{
    public void Configure(EntityTypeBuilder<AnatomiaChunkVector> builder)
    {
        builder.ToTable("anatomia_chunks");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Texto).IsRequired();
        builder.Property(x => x.Secao).HasMaxLength(300);
        builder.Property(x => x.Assunto).HasMaxLength(200);
        builder.Property(x => x.Subassunto).HasMaxLength(200);
        builder.Property(x => x.Embedding).HasColumnType("extensions.vector(1536)").IsRequired();
        builder.HasIndex(x => new { x.FonteAnatomiaId, x.Pagina });
        builder.HasIndex(x => x.Embedding)
            .HasMethod("hnsw")
            .HasOperators("extensions.vector_cosine_ops")
            .HasStorageParameter("m", 16)
            .HasStorageParameter("ef_construction", 64);
        builder.HasOne(x => x.FonteAnatomia).WithMany(x => x.Chunks)
            .HasForeignKey(x => x.FonteAnatomiaId).OnDelete(DeleteBehavior.Cascade);
    }
}
