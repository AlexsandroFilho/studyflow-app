using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyFlow.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddResumoTema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "resumos_nota");

            migrationBuilder.CreateTable(
                name: "resumos_tema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TemaId = table.Column<int>(type: "integer", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ResultadoJson = table.Column<string>(type: "text", nullable: false),
                    Modelo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_resumos_tema", x => x.Id);
                    table.ForeignKey(
                        name: "FK_resumos_tema_temas_TemaId",
                        column: x => x.TemaId,
                        principalTable: "temas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_resumos_tema_usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_resumos_tema_TemaId_DataCriacao",
                table: "resumos_tema",
                columns: new[] { "TemaId", "DataCriacao" });

            migrationBuilder.CreateIndex(
                name: "IX_resumos_tema_UsuarioId",
                table: "resumos_tema",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "resumos_tema");

            migrationBuilder.CreateTable(
                name: "resumos_nota",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NotaId = table.Column<int>(type: "integer", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Modelo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ResultadoJson = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_resumos_nota", x => x.Id);
                    table.ForeignKey(
                        name: "FK_resumos_nota_notas_NotaId",
                        column: x => x.NotaId,
                        principalTable: "notas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_resumos_nota_usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_resumos_nota_NotaId_DataCriacao",
                table: "resumos_nota",
                columns: new[] { "NotaId", "DataCriacao" });

            migrationBuilder.CreateIndex(
                name: "IX_resumos_nota_UsuarioId",
                table: "resumos_nota",
                column: "UsuarioId");
        }
    }
}
