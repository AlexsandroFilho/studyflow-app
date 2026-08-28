using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyFlow.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddUsuarioAndTemaOptional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_notas_temas_TemaId",
                table: "notas");

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioId",
                table: "temas",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<int>(
                name: "TemaId",
                table: "notas",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioId",
                table: "notas",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SenhaHash = table.Column<string>(type: "text", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_temas_UsuarioId",
                table: "temas",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_notas_UsuarioId",
                table: "notas",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_Email",
                table: "usuarios",
                column: "Email",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_notas_temas_TemaId",
                table: "notas",
                column: "TemaId",
                principalTable: "temas",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_notas_temas_TemaId",
                table: "notas");

            migrationBuilder.DropForeignKey(
                name: "FK_notas_usuarios_UsuarioId",
                table: "notas");

            migrationBuilder.DropForeignKey(
                name: "FK_temas_usuarios_UsuarioId",
                table: "temas");

            migrationBuilder.DropTable(
                name: "usuarios");

            migrationBuilder.DropIndex(
                name: "IX_temas_UsuarioId",
                table: "temas");

            migrationBuilder.DropIndex(
                name: "IX_notas_UsuarioId",
                table: "notas");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "temas");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "notas");

            migrationBuilder.AlterColumn<int>(
                name: "TemaId",
                table: "notas",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_notas_temas_TemaId",
                table: "notas",
                column: "TemaId",
                principalTable: "temas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
