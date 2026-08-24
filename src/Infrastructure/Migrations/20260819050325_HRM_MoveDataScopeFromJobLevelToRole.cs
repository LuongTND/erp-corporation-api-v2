using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class HRM_MoveDataScopeFromJobLevelToRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultScopeType",
                table: "JobLevels");

            migrationBuilder.AddColumn<string>(
                name: "DefaultDataScope",
                table: "Roles",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "Own");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultDataScope",
                table: "Roles");

            migrationBuilder.AddColumn<string>(
                name: "DefaultScopeType",
                table: "JobLevels",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "Own");
        }
    }
}
