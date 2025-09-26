using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetoChallengeMottu.Migrations
{
    /// <inheritdoc />
    public partial class AddLocalizacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LOCALIZACOES",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    MotoId = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    EchoBeaconId = table.Column<long>(type: "NUMBER(19)", nullable: true),
                    Setor = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    Status = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    DataHoraRegistro = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LOCALIZACOES", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LOCALIZACOES_ECHOBEACON_EchoBeaconId",
                        column: x => x.EchoBeaconId,
                        principalTable: "ECHOBEACON",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_LOCALIZACOES_MOTOS_MotoId",
                        column: x => x.MotoId,
                        principalTable: "MOTOS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ECHOBEACON_NumeroIdentificacao",
                table: "ECHOBEACON",
                column: "NumeroIdentificacao",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LOCALIZACOES_EchoBeaconId",
                table: "LOCALIZACOES",
                column: "EchoBeaconId");

            migrationBuilder.CreateIndex(
                name: "IX_LOCALIZACOES_MotoId",
                table: "LOCALIZACOES",
                column: "MotoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LOCALIZACOES");

            migrationBuilder.DropIndex(
                name: "IX_ECHOBEACON_NumeroIdentificacao",
                table: "ECHOBEACON");
        }
    }
}
