using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymLinkPro.Data.Migrations
{
    /// <inheritdoc />
    public partial class _12AugUpdate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ProjectMembers_ProjectId",
                table: "ProjectMembers",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectLinks_ProjectId",
                table: "ProjectLinks",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassRegistrations_ClassId",
                table: "ClassRegistrations",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassRegistrations_MemberId",
                table: "ClassRegistrations",
                column: "MemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassRegistrations_GymClasses_ClassId",
                table: "ClassRegistrations",
                column: "ClassId",
                principalTable: "GymClasses",
                principalColumn: "GymClassId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassRegistrations_Users_MemberId",
                table: "ClassRegistrations",
                column: "MemberId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectLinks_Projects_ProjectId",
                table: "ProjectLinks",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "ProjectId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectMembers_Projects_ProjectId",
                table: "ProjectMembers",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "ProjectId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassRegistrations_GymClasses_ClassId",
                table: "ClassRegistrations");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassRegistrations_Users_MemberId",
                table: "ClassRegistrations");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectLinks_Projects_ProjectId",
                table: "ProjectLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectMembers_Projects_ProjectId",
                table: "ProjectMembers");

            migrationBuilder.DropIndex(
                name: "IX_ProjectMembers_ProjectId",
                table: "ProjectMembers");

            migrationBuilder.DropIndex(
                name: "IX_ProjectLinks_ProjectId",
                table: "ProjectLinks");

            migrationBuilder.DropIndex(
                name: "IX_ClassRegistrations_ClassId",
                table: "ClassRegistrations");

            migrationBuilder.DropIndex(
                name: "IX_ClassRegistrations_MemberId",
                table: "ClassRegistrations");
        }
    }
}
