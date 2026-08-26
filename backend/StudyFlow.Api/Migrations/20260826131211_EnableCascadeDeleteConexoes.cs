using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyFlow.Api.Migrations
{
    /// <inheritdoc />
    public partial class EnableCascadeDeleteConexoes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_nota_conexoes_notas_NotaDestinoId",
                table: "nota_conexoes");

            migrationBuilder.DropForeignKey(
                name: "FK_nota_conexoes_notas_NotaOrigemId",
                table: "nota_conexoes");

            migrationBuilder.AddForeignKey(
                name: "FK_nota_conexoes_notas_NotaDestinoId",
                table: "nota_conexoes",
                column: "NotaDestinoId",
                principalTable: "notas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_nota_conexoes_notas_NotaOrigemId",
                table: "nota_conexoes",
                column: "NotaOrigemId",
                principalTable: "notas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_nota_conexoes_notas_NotaDestinoId",
                table: "nota_conexoes");

            migrationBuilder.DropForeignKey(
                name: "FK_nota_conexoes_notas_NotaOrigemId",
                table: "nota_conexoes");

            migrationBuilder.AddForeignKey(
                name: "FK_nota_conexoes_notas_NotaDestinoId",
                table: "nota_conexoes",
                column: "NotaDestinoId",
                principalTable: "notas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_nota_conexoes_notas_NotaOrigemId",
                table: "nota_conexoes",
                column: "NotaOrigemId",
                principalTable: "notas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
