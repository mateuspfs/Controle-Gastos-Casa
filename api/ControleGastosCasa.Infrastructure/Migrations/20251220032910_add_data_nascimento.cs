using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControleGastosCasa.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_data_nascimento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Idade",
                table: "pessoas");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataNascimento",
                table: "pessoas",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataNascimento",
                table: "pessoas");

            migrationBuilder.AddColumn<int>(
                name: "Idade",
                table: "pessoas",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
