using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Sprint2_RequestCode_Level1Note : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Level1Note",
                table: "RecruitmentRequests",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequestCode",
                table: "RecruitmentRequests",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_RecruitmentRequests_RequestCode",
                table: "RecruitmentRequests",
                column: "RequestCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RecruitmentRequests_RequestCode",
                table: "RecruitmentRequests");

            migrationBuilder.DropColumn(
                name: "Level1Note",
                table: "RecruitmentRequests");

            migrationBuilder.DropColumn(
                name: "RequestCode",
                table: "RecruitmentRequests");
        }
    }
}
