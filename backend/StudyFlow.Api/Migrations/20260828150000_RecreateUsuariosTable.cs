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
            migrationBuilder.Sql("""
                DO $migration$
                BEGIN
                    IF to_regclass('public.usuarios') IS NULL THEN
                        CREATE TABLE usuarios (
                            "Id" uuid NOT NULL,
                            "Nome" character varying(150) NOT NULL,
                            "Email" character varying(200) NOT NULL,
                            "SenhaHash" text NOT NULL,
                            "DataCriacao" timestamp with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
                            "Role" integer NOT NULL DEFAULT 2,
                            CONSTRAINT "PK_usuarios" PRIMARY KEY ("Id")
                        );

                        CREATE UNIQUE INDEX "IX_usuarios_Email" ON usuarios ("Email");

                        INSERT INTO usuarios ("Id", "Nome", "Email", "SenhaHash", "DataCriacao", "Role")
                        VALUES ('00000000-0000-0000-0000-000000000000', 'Usuário legado', 'legacy@studyflow.invalid', '', CURRENT_TIMESTAMP, 2);

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

                        ALTER TABLE notas
                            ADD CONSTRAINT "FK_notas_usuarios_UsuarioId"
                            FOREIGN KEY ("UsuarioId") REFERENCES usuarios ("Id") ON DELETE RESTRICT;

                        ALTER TABLE temas
                            ADD CONSTRAINT "FK_temas_usuarios_UsuarioId"
                            FOREIGN KEY ("UsuarioId") REFERENCES usuarios ("Id") ON DELETE RESTRICT;
                    END IF;
                END $migration$;
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // This migration only repairs a manually deleted table, so it must not
            // remove the usuarios table created by the original migrations.
        }
    }
}
