using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class HRM023_MoveResignFieldsToEmploymentInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HandoverCompleted",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ResignedAt",
                table: "Users");

            migrationBuilder.AddColumn<bool>(
                name: "HandoverCompleted",
                table: "EmploymentInfos",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ResignedAt",
                table: "EmploymentInfos",
                type: "datetimeoffset",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HandoverCompleted",
                table: "EmploymentInfos");

            migrationBuilder.DropColumn(
                name: "ResignedAt",
                table: "EmploymentInfos");

            migrationBuilder.AddColumn<bool>(
                name: "HandoverCompleted",
                table: "Users",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ResignedAt",
                table: "Users",
                type: "datetimeoffset",
                nullable: true);
        }
    }
}
