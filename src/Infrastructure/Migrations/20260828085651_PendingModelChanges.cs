using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PendingModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Level1ApprovedAt",
                table: "RecruitmentRequests",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Level1ApproverId",
                table: "RecruitmentRequests",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Level2ApprovedAt",
                table: "RecruitmentRequests",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Level2ApproverId",
                table: "RecruitmentRequests",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "TrialStartDate",
                table: "Candidates",
                type: "date",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "InterviewRuleConfigs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Context = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RegionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InterviewerRoleKey = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SchedulerRoleKey = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NotifyRoleKey = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewRuleConfigs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterviewRuleConfigs_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_InterviewRuleConfigs_Regions_RegionId",
                        column: x => x.RegionId,
                        principalTable: "Regions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "InterviewSchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CandidateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InterviewerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScheduledAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LocationNote = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    InterviewResult = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CompletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterviewSchedules_Candidates_CandidateId",
                        column: x => x.CandidateId,
                        principalTable: "Candidates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InterviewSchedules_Users_InterviewerId",
                        column: x => x.InterviewerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecruitmentRequests_Level1ApproverId",
                table: "RecruitmentRequests",
                column: "Level1ApproverId");

            migrationBuilder.CreateIndex(
                name: "IX_RecruitmentRequests_Level2ApproverId",
                table: "RecruitmentRequests",
                column: "Level2ApproverId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewRuleConfigs_Context_RegionId_IsActive",
                table: "InterviewRuleConfigs",
                columns: new[] { "Context", "RegionId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_InterviewRuleConfigs_DepartmentId",
                table: "InterviewRuleConfigs",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewRuleConfigs_RegionId",
                table: "InterviewRuleConfigs",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewSchedules_CandidateId",
                table: "InterviewSchedules",
                column: "CandidateId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewSchedules_InterviewerId_ScheduledAt",
                table: "InterviewSchedules",
                columns: new[] { "InterviewerId", "ScheduledAt" });

            migrationBuilder.AddForeignKey(
                name: "FK_RecruitmentRequests_Users_Level1ApproverId",
                table: "RecruitmentRequests",
                column: "Level1ApproverId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RecruitmentRequests_Users_Level2ApproverId",
                table: "RecruitmentRequests",
                column: "Level2ApproverId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecruitmentRequests_Users_Level1ApproverId",
                table: "RecruitmentRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_RecruitmentRequests_Users_Level2ApproverId",
                table: "RecruitmentRequests");

            migrationBuilder.DropTable(
                name: "InterviewRuleConfigs");

            migrationBuilder.DropTable(
                name: "InterviewSchedules");

            migrationBuilder.DropIndex(
                name: "IX_RecruitmentRequests_Level1ApproverId",
                table: "RecruitmentRequests");

            migrationBuilder.DropIndex(
                name: "IX_RecruitmentRequests_Level2ApproverId",
                table: "RecruitmentRequests");

            migrationBuilder.DropColumn(
                name: "Level1ApprovedAt",
                table: "RecruitmentRequests");

            migrationBuilder.DropColumn(
                name: "Level1ApproverId",
                table: "RecruitmentRequests");

            migrationBuilder.DropColumn(
                name: "Level2ApprovedAt",
                table: "RecruitmentRequests");

            migrationBuilder.DropColumn(
                name: "Level2ApproverId",
                table: "RecruitmentRequests");

            migrationBuilder.DropColumn(
                name: "TrialStartDate",
                table: "Candidates");
        }
    }
}
