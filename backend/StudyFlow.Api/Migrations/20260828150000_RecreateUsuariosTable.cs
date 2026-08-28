using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using StudyFlow.Api.Data;

#nullable disable

namespace StudyFlow.Api.Migrations
{
    /// <summary>
    /// Recreates the usuarios table after it was removed manually from PostgreSQL
    /// while EF Core still considered the original migrations applied.
    /// </summary>
    [DbContext(typeof(AppDbContext))]
    [Migration("20260828150000_RecreateUsuariosTable")]
    public partial class RecreateUsuariosTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SenhaHash = table.Column<string>(type: "text", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Role = table.Column<int>(type: "integer", nullable: false, defaultValue: 2)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.Id);
                });

            // Keeps existing temas/notas with the old default UsuarioId valid.
            migrationBuilder.Sql("""
                INSERT INTO usuarios ("Id", "Nome", "Email", "SenhaHash", "DataCriacao", "Role")
                VALUES ('00000000-0000-0000-0000-000000000000', 'Usuário legado', 'legacy@studyflow.invalid', '', CURRENT_TIMESTAMP, 2)
                ON CONFLICT ("Id") DO NOTHING;
                """);

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_Email",
                table: "usuarios",
                column: "Email",
                unique: true);

            // Existing rows may reference users that were deleted with the table.
            // Associate those orphaned rows with the legacy placeholder before
            // restoring the foreign-key constraints.
            migrationBuilder.Sql("""
                UPDATE temas
                SET "UsuarioId" = '00000000-0000-0000-0000-000000000000'
                WHERE NOT EXISTS (
                    SELECT 1 FROM usuarios u WHERE u."Id" = temas."UsuarioId"
                );

                UPDATE notas
                SET "UsuarioId" = '00000000-0000-0000-0000-000000000000'
                WHERE NOT EXISTS (
                    SELECT 1 FROM usuarios u WHERE u."Id" = notas."UsuarioId"
                );
                """);

            migrationBuilder.AddForeignKey(
                name: "FK_notas_usuarios_UsuarioId",
                table: "notas",
                column: "UsuarioId",
                principalTable: "usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_temas_usuarios_UsuarioId",
                table: "temas",
                column: "UsuarioId",
                principalTable: "usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey("FK_notas_usuarios_UsuarioId", "notas");
            migrationBuilder.DropForeignKey("FK_temas_usuarios_UsuarioId", "temas");
            migrationBuilder.DropTable("usuarios");
        }
    }
}
