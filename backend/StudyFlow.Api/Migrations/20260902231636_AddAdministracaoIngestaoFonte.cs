using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyFlow.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddAdministracaoIngestaoFonte : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ingestoes_fontes_anatomia",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    FonteAnatomiaId = table.Column<Guid>(type: "uuid", nullable: true),
                    Titulo = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Autor = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Versao = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Assunto = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Subassunto = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ArquivoTemporarioChave = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    MensagemErro = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    QuantidadeChunks = table.Column<int>(type: "integer", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DataConclusao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ingestoes_fontes_anatomia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ingestoes_fontes_anatomia_fontes_anatomia_FonteAnatomiaId",
                        column: x => x.FonteAnatomiaId,
                        principalTable: "fontes_anatomia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ingestoes_fontes_anatomia_usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ingestoes_fontes_anatomia_FonteAnatomiaId",
                table: "ingestoes_fontes_anatomia",
                column: "FonteAnatomiaId");

            migrationBuilder.CreateIndex(
                name: "IX_ingestoes_fontes_anatomia_Status_DataCriacao",
                table: "ingestoes_fontes_anatomia",
                columns: new[] { "Status", "DataCriacao" });

            migrationBuilder.CreateIndex(
                name: "IX_ingestoes_fontes_anatomia_UsuarioId",
                table: "ingestoes_fontes_anatomia",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ingestoes_fontes_anatomia");
        }
    }
}
