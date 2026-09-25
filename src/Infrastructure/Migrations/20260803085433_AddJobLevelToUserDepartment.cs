using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddJobLevelToUserDepartment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "JobLevelId",
                table: "UserDepartments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserDepartments_JobLevelId",
                table: "UserDepartments",
                column: "JobLevelId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserDepartments_JobLevels_JobLevelId",
                table: "UserDepartments",
                column: "JobLevelId",
                principalTable: "JobLevels",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserDepartments_JobLevels_JobLevelId",
                table: "UserDepartments");

            migrationBuilder.DropIndex(
                name: "IX_UserDepartments_JobLevelId",
                table: "UserDepartments");

            migrationBuilder.DropColumn(
                name: "JobLevelId",
                table: "UserDepartments");
        }
    }
}
