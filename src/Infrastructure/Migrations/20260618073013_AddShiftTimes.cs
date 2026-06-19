using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddShiftTimes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CheckInTimeTarget",
                table: "Users",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CheckOutTimeTarget",
                table: "Users",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CheckInTimeTarget",
                table: "Departments",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: false,
                defaultValue: "08:00");

            migrationBuilder.AddColumn<string>(
                name: "CheckOutTimeTarget",
                table: "Departments",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: false,
                defaultValue: "17:00");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CheckInTimeTarget",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CheckOutTimeTarget",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CheckInTimeTarget",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "CheckOutTimeTarget",
                table: "Departments");
        }
    }
}
