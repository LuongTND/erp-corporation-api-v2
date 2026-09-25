using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class HRM_DropUserDepartmentJobLevelAndStatusHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserDepartments_JobLevels_JobLevelId",
                table: "UserDepartments");

            migrationBuilder.DropTable(
                name: "UserStatusHistories");

            migrationBuilder.DropIndex(
                name: "IX_UserDepartments_JobLevelId",
                table: "UserDepartments");

            migrationBuilder.DropColumn(
                name: "JobLevelId",
                table: "UserDepartments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "JobLevelId",
                table: "UserDepartments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserStatusHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChangedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ChangedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    NewStatus = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OldStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserStatusHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserStatusHistories_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserDepartments_JobLevelId",
                table: "UserDepartments",
                column: "JobLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_UserStatusHistories_UserId",
                table: "UserStatusHistories",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserDepartments_JobLevels_JobLevelId",
                table: "UserDepartments",
                column: "JobLevelId",
                principalTable: "JobLevels",
                principalColumn: "Id");
        }
    }
}
