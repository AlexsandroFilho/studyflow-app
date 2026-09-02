using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyFlow.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddQuizTema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "quizzes_tema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TemaId = table.Column<int>(type: "integer", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Mensagem = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Modelo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quizzes_tema", x => x.Id);
                    table.ForeignKey(
                        name: "FK_quizzes_tema_temas_TemaId",
                        column: x => x.TemaId,
                        principalTable: "temas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_quizzes_tema_usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "quiz_tema_perguntas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    QuizTemaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Ordem = table.Column<int>(type: "integer", nullable: false),
                    Enunciado = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    AlternativasJson = table.Column<string>(type: "text", nullable: false),
                    IndiceRespostaCorreta = table.Column<int>(type: "integer", nullable: false),
                    Explicacao = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    ReferenciasJson = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quiz_tema_perguntas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_quiz_tema_perguntas_quizzes_tema_QuizTemaId",
                        column: x => x.QuizTemaId,
                        principalTable: "quizzes_tema",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tentativas_quiz_tema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    QuizTemaId = table.Column<Guid>(type: "uuid", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuantidadeAcertos = table.Column<int>(type: "integer", nullable: false),
                    QuantidadeQuestoes = table.Column<int>(type: "integer", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tentativas_quiz_tema", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tentativas_quiz_tema_quizzes_tema_QuizTemaId",
                        column: x => x.QuizTemaId,
                        principalTable: "quizzes_tema",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tentativas_quiz_tema_usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "respostas_tentativa_quiz_tema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TentativaQuizTemaId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuizTemaPerguntaId = table.Column<Guid>(type: "uuid", nullable: false),
                    IndiceAlternativaSelecionada = table.Column<int>(type: "integer", nullable: false),
                    Acertou = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_respostas_tentativa_quiz_tema", x => x.Id);
                    table.ForeignKey(
                        name: "FK_respostas_tentativa_quiz_tema_quiz_tema_perguntas_QuizTemaP~",
                        column: x => x.QuizTemaPerguntaId,
                        principalTable: "quiz_tema_perguntas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_respostas_tentativa_quiz_tema_tentativas_quiz_tema_Tentativ~",
                        column: x => x.TentativaQuizTemaId,
                        principalTable: "tentativas_quiz_tema",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_quiz_tema_perguntas_QuizTemaId_Ordem",
                table: "quiz_tema_perguntas",
                columns: new[] { "QuizTemaId", "Ordem" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_quizzes_tema_TemaId_DataCriacao",
                table: "quizzes_tema",
                columns: new[] { "TemaId", "DataCriacao" });

            migrationBuilder.CreateIndex(
                name: "IX_quizzes_tema_UsuarioId",
                table: "quizzes_tema",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_respostas_tentativa_quiz_tema_QuizTemaPerguntaId",
                table: "respostas_tentativa_quiz_tema",
                column: "QuizTemaPerguntaId");

            migrationBuilder.CreateIndex(
                name: "IX_respostas_tentativa_quiz_tema_TentativaQuizTemaId_QuizTemaP~",
                table: "respostas_tentativa_quiz_tema",
                columns: new[] { "TentativaQuizTemaId", "QuizTemaPerguntaId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tentativas_quiz_tema_QuizTemaId_DataCriacao",
                table: "tentativas_quiz_tema",
                columns: new[] { "QuizTemaId", "DataCriacao" });

            migrationBuilder.CreateIndex(
                name: "IX_tentativas_quiz_tema_UsuarioId",
                table: "tentativas_quiz_tema",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "respostas_tentativa_quiz_tema");

            migrationBuilder.DropTable(
                name: "quiz_tema_perguntas");

            migrationBuilder.DropTable(
                name: "tentativas_quiz_tema");

            migrationBuilder.DropTable(
                name: "quizzes_tema");
        }
    }
}
