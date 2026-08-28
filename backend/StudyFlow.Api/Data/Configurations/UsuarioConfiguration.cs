using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudyFlow.Api.Domain.Entities;
using StudyFlow.Api.Domain.Enums;

namespace StudyFlow.Api.Data.Configurations
{
    public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("usuarios");
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Nome)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(200);

            builder.HasIndex(u => u.Email)
                .IsUnique()
                .HasDatabaseName("IX_usuarios_Email");

            builder.Property(u => u.SenhaHash)
                .IsRequired();

            builder.Property(u => u.DataCriacao)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(u => u.Role)
                .HasConversion<int>()
                .HasDefaultValue(UserRole.User)
                .IsRequired();
        }
    }
}
