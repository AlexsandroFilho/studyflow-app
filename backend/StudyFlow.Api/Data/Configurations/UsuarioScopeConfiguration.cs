using Microsoft.EntityFrameworkCore;
using StudyFlow.Api.Data;

namespace StudyFlow.Api.Data.Configurations;

public class UsuarioScopeConfiguration
{
    public void Configure(ModelBuilder modelBuilder, AppDbContext dbContext)
    {
        modelBuilder.Entity<Domain.Entities.Tema>()
            .HasQueryFilter(tema =>
                dbContext.CurrentUsuarioId.HasValue &&
                tema.UsuarioId == dbContext.CurrentUsuarioId.Value);

        modelBuilder.Entity<Domain.Entities.Nota>()
            .HasQueryFilter(nota =>
                dbContext.CurrentUsuarioId.HasValue &&
                nota.UsuarioId == dbContext.CurrentUsuarioId.Value);

        modelBuilder.Entity<Domain.Entities.ConexaoNota>()
            .HasQueryFilter(conexao =>
                dbContext.CurrentUsuarioId.HasValue &&
                conexao.NotaOrigem!.UsuarioId == dbContext.CurrentUsuarioId.Value &&
                conexao.NotaDestino!.UsuarioId == dbContext.CurrentUsuarioId.Value);
    }
}