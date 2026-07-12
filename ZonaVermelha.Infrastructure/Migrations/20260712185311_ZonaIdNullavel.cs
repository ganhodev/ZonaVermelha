using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZonaVermelha.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ZonaIdNullavel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Relatos_Zonas_ZonaId",
                table: "Relatos");

            migrationBuilder.AlterColumn<Guid>(
                name: "ZonaId",
                table: "Relatos",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AddForeignKey(
                name: "FK_Relatos_Zonas_ZonaId",
                table: "Relatos",
                column: "ZonaId",
                principalTable: "Zonas",
                principalColumn: "IdZona");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Relatos_Zonas_ZonaId",
                table: "Relatos");

            migrationBuilder.AlterColumn<Guid>(
                name: "ZonaId",
                table: "Relatos",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Relatos_Zonas_ZonaId",
                table: "Relatos",
                column: "ZonaId",
                principalTable: "Zonas",
                principalColumn: "IdZona",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
