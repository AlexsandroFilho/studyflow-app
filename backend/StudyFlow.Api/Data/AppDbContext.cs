using Microsoft.EntityFrameworkCore;
using Pgvector.EntityFrameworkCore;
using StudyFlow.Api.Domain.Entities;
using StudyFlow.Api.Data.Configurations;

namespace StudyFlow.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public Guid? CurrentUsuarioId { get; set; }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Tema> Temas { get; set; }
        public DbSet<Nota> Notas { get; set; }
        public DbSet<ConexaoNota> ConexaoNotas { get; set; }
        public DbSet<FonteAnatomia> FontesAnatomia { get; set; }
        public DbSet<AnatomiaChunkVector> AnatomiaChunks { get; set; }
        public DbSet<RevisaoNota> RevisoesNota { get; set; }
        public DbSet<ResumoTema> ResumosTema { get; set; }
        public DbSet<QuizTema> QuizzesTema { get; set; }
        public DbSet<QuizTemaPergunta> QuizTemaPerguntas { get; set; }
        public DbSet<TentativaQuizTema> TentativasQuizTema { get; set; }
        public DbSet<RespostaTentativaQuizTema> RespostasTentativaQuizTema { get; set; }
        public DbSet<IngestaoFonteAnatomia> IngestoesFontesAnatomia { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasPostgresExtension("extensions", "vector", null);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            new UsuarioScopeConfiguration().Configure(modelBuilder, this);
        }
    }
}
