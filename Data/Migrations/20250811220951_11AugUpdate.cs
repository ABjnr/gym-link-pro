using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymLinkPro.Data.Migrations
{
    /// <inheritdoc />
    public partial class _11AugUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "GymClasses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndTime",
                table: "GymClasses",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Instructor",
                table: "GymClasses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartTime",
                table: "GymClasses",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "GymClasses");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "GymClasses");

            migrationBuilder.DropColumn(
                name: "Instructor",
                table: "GymClasses");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "GymClasses");
        }
    }
}
