using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Pgvector;

#nullable disable

namespace StudyFlow.Api.Migrations
{
    public partial class AddAnatomiaRag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS vector WITH SCHEMA extensions;");

            migrationBuilder.CreateTable(
                name: "fontes_anatomia",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Titulo = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Autor = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Versao = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ArquivoChave = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    HashConteudo = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Publicada = table.Column<bool>(type: "boolean", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table => table.PrimaryKey("PK_fontes_anatomia", x => x.Id));

            migrationBuilder.CreateTable(
                name: "revisoes_nota",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NotaId = table.Column<int>(type: "integer", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ResultadoJson = table.Column<string>(type: "text", nullable: false),
                    Modelo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_revisoes_nota", x => x.Id);
                    table.ForeignKey("FK_revisoes_nota_notas_NotaId", x => x.NotaId, "notas", "Id", onDelete: ReferentialAction.Cascade);
                    table.ForeignKey("FK_revisoes_nota_usuarios_UsuarioId", x => x.UsuarioId, "usuarios", "Id", onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "anatomia_chunks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FonteAnatomiaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Texto = table.Column<string>(type: "text", nullable: false),
                    Pagina = table.Column<int>(type: "integer", nullable: false),
                    Secao = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Assunto = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Subassunto = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Embedding = table.Column<Vector>(type: "extensions.vector(1536)", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_anatomia_chunks", x => x.Id);
                    table.ForeignKey("FK_anatomia_chunks_fontes_anatomia_FonteAnatomiaId", x => x.FonteAnatomiaId, "fontes_anatomia", "Id", onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex("IX_fontes_anatomia_HashConteudo", "fontes_anatomia", "HashConteudo", unique: true);
            migrationBuilder.CreateIndex("IX_revisoes_nota_NotaId_DataCriacao", "revisoes_nota", new[] { "NotaId", "DataCriacao" });
            migrationBuilder.CreateIndex("IX_revisoes_nota_UsuarioId", "revisoes_nota", "UsuarioId");
            migrationBuilder.CreateIndex("IX_anatomia_chunks_FonteAnatomiaId_Pagina", "anatomia_chunks", new[] { "FonteAnatomiaId", "Pagina" });
            migrationBuilder.CreateIndex("IX_anatomia_chunks_Embedding", "anatomia_chunks", "Embedding")
                .Annotation("Npgsql:IndexMethod", "hnsw")
                .Annotation("Npgsql:IndexOperators", new[] { "extensions.vector_cosine_ops" })
                .Annotation("Npgsql:StorageParameter:ef_construction", 64)
                .Annotation("Npgsql:StorageParameter:m", 16);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "anatomia_chunks");
            migrationBuilder.DropTable(name: "revisoes_nota");
            migrationBuilder.DropTable(name: "fontes_anatomia");
        }
    }
}
