using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Animed.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class initdb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tb_net_tutor",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Nome = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Cpf = table.Column<string>(type: "NVARCHAR2(14)", maxLength: 14, nullable: false),
                    Telefone = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Cep = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    PontosTotais = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Nivel = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_net_tutor", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tb_net_usuario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Nome = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Email = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Password = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Role = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_net_usuario", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tb_net_pet",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Nome = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Especie = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Raca = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    DataNascimento = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    PesoKg = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    Castrado = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    TutorId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_net_pet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tb_net_pet_tb_net_tutor_TutorId",
                        column: x => x.TutorId,
                        principalTable: "tb_net_tutor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tb_net_consulta",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    DataHora = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Motivo = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    Diagnostico = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Prescricao = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    PetId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_net_consulta", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tb_net_consulta_tb_net_pet_PetId",
                        column: x => x.PetId,
                        principalTable: "tb_net_pet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tb_net_vacina",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NomeVacina = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    DataAplicacao = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DataProximaDose = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    PetId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_net_vacina", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tb_net_vacina_tb_net_pet_PetId",
                        column: x => x.PetId,
                        principalTable: "tb_net_pet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "tb_net_usuario",
                columns: new[] { "Id", "Email", "Nome", "Password", "Role" },
                values: new object[,]
                {
                    { 1, "admin@animed.com.br", "Administrador", "animed123", "admin" },
                    { 2, "doutor@animed.com.br", "Dra. Helena Prado", "animed123", "veterinario" },
                    { 3, "tutor@animed.com.br", "Tutor Demonstração", "animed123", "tutor" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_tb_net_consulta_PetId",
                table: "tb_net_consulta",
                column: "PetId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_net_pet_TutorId_Nome",
                table: "tb_net_pet",
                columns: new[] { "TutorId", "Nome" });

            migrationBuilder.CreateIndex(
                name: "IDX_NET_TUTOR_EMAIL",
                table: "tb_net_tutor",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IDX_NET_USUARIO_EMAIL",
                table: "tb_net_usuario",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tb_net_vacina_PetId",
                table: "tb_net_vacina",
                column: "PetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tb_net_consulta");

            migrationBuilder.DropTable(
                name: "tb_net_usuario");

            migrationBuilder.DropTable(
                name: "tb_net_vacina");

            migrationBuilder.DropTable(
                name: "tb_net_pet");

            migrationBuilder.DropTable(
                name: "tb_net_tutor");
        }
    }
}
