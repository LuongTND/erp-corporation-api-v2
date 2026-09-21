using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRoundTypeAndStepName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Label",
                table: "InterviewRuleConfigSteps");

            migrationBuilder.RenameColumn(
                name: "Round",
                table: "InterviewSchedules",
                newName: "RoundNumber");

            migrationBuilder.AlterColumn<Guid>(
                name: "InterviewerId",
                table: "InterviewSchedules",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "InterviewRuleConfigSteps",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RoundTypeId",
                table: "InterviewRuleConfigSteps",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "InterviewScheduleId1",
                table: "ApplicationEvaluations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RoundTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsSystem = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoundTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InterviewRuleConfigSteps_RoundTypeId",
                table: "InterviewRuleConfigSteps",
                column: "RoundTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationEvaluations_InterviewScheduleId1",
                table: "ApplicationEvaluations",
                column: "InterviewScheduleId1");

            migrationBuilder.CreateIndex(
                name: "IX_RoundTypes_Name",
                table: "RoundTypes",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationEvaluations_InterviewSchedules_InterviewScheduleId1",
                table: "ApplicationEvaluations",
                column: "InterviewScheduleId1",
                principalTable: "InterviewSchedules",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InterviewRuleConfigSteps_RoundTypes_RoundTypeId",
                table: "InterviewRuleConfigSteps",
                column: "RoundTypeId",
                principalTable: "RoundTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationEvaluations_InterviewSchedules_InterviewScheduleId1",
                table: "ApplicationEvaluations");

            migrationBuilder.DropForeignKey(
                name: "FK_InterviewRuleConfigSteps_RoundTypes_RoundTypeId",
                table: "InterviewRuleConfigSteps");

            migrationBuilder.DropTable(
                name: "RoundTypes");

            migrationBuilder.DropIndex(
                name: "IX_InterviewRuleConfigSteps_RoundTypeId",
                table: "InterviewRuleConfigSteps");

            migrationBuilder.DropIndex(
                name: "IX_ApplicationEvaluations_InterviewScheduleId1",
                table: "ApplicationEvaluations");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "InterviewRuleConfigSteps");

            migrationBuilder.DropColumn(
                name: "RoundTypeId",
                table: "InterviewRuleConfigSteps");

            migrationBuilder.DropColumn(
                name: "InterviewScheduleId1",
                table: "ApplicationEvaluations");

            migrationBuilder.RenameColumn(
                name: "RoundNumber",
                table: "InterviewSchedules",
                newName: "Round");

            migrationBuilder.AlterColumn<Guid>(
                name: "InterviewerId",
                table: "InterviewSchedules",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Label",
                table: "InterviewRuleConfigSteps",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }
    }
}
