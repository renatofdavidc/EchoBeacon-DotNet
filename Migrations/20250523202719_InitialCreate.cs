using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetoChallengeMottu.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MOTOS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Placa = table.Column<string>(type: "NVARCHAR2(10)", maxLength: 10, nullable: false),
                    Modelo = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MOTOS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ECHOBEACON",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NumeroIdentificacao = table.Column<string>(type: "NVARCHAR2(10)", maxLength: 10, nullable: false),
                    DataRegistro = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    MotoId = table.Column<long>(type: "NUMBER(19)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ECHOBEACON", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ECHOBEACON_MOTOS_MotoId",
                        column: x => x.MotoId,
                        principalTable: "MOTOS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ECHOBEACON_MotoId",
                table: "ECHOBEACON",
                column: "MotoId",
                unique: true,
                filter: "\"MotoId\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ECHOBEACON");

            migrationBuilder.DropTable(
                name: "MOTOS");
        }
    }
}
