using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymLinkPro.Data.Migrations
{
    /// <inheritdoc />
    public partial class addedByUserIdAndCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "ProjectMembers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "AddedByUserId",
                table: "ProjectLinks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "ProjectLinks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "ProjectMembers");

            migrationBuilder.DropColumn(
                name: "AddedByUserId",
                table: "ProjectLinks");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "ProjectLinks");
        }
    }
}
