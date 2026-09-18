using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RecruitmentSchemaRedesign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InterviewSchedules_Candidates_CandidateId",
                table: "InterviewSchedules");

            migrationBuilder.DropTable(
                name: "CandidateEvaluations");

            migrationBuilder.DropTable(
                name: "RecruitmentApproverConfigs");

            migrationBuilder.DropTable(
                name: "Candidates");

            migrationBuilder.DropIndex(
                name: "IX_JobPostings_RecruitmentRequestId",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "InterviewerRoleKey",
                table: "InterviewRuleConfigs");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "InterviewRuleConfigs");

            migrationBuilder.DropColumn(
                name: "SchedulerRoleKey",
                table: "InterviewRuleConfigs");

            migrationBuilder.RenameColumn(
                name: "CandidateId",
                table: "InterviewSchedules",
                newName: "ApplicationId");

            migrationBuilder.RenameIndex(
                name: "IX_InterviewSchedules_CandidateId",
                table: "InterviewSchedules",
                newName: "IX_InterviewSchedules_ApplicationId");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "JobPostings",
                type: "nvarchar(max)",
                maxLength: 8000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JobType",
                table: "JobPostings",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Requirements",
                table: "JobPostings",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SalaryMax",
                table: "JobPostings",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SalaryMin",
                table: "JobPostings",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SalaryVisible",
                table: "JobPostings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "JobPostings",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WorkingLocation",
                table: "JobPostings",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Round",
                table: "InterviewSchedules",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AlterColumn<string>(
                name: "NotifyRoleKey",
                table: "InterviewRuleConfigs",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.CreateTable(
                name: "Applicants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applicants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InterviewRuleConfigSteps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InterviewRuleConfigId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoundNumber = table.Column<int>(type: "int", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    InterviewerRoleKey = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SchedulerRoleKey = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewRuleConfigSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterviewRuleConfigSteps_InterviewRuleConfigs_InterviewRuleConfigId",
                        column: x => x.InterviewRuleConfigId,
                        principalTable: "InterviewRuleConfigs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApplicantDocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FileUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicantDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicantDocuments_Applicants_ApplicantId",
                        column: x => x.ApplicantId,
                        principalTable: "Applicants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Applications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RecruitmentRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    JobPostingId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SourceChannel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Stage = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RejectionReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TrialStartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ConvertedEmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Applications_Applicants_ApplicantId",
                        column: x => x.ApplicantId,
                        principalTable: "Applicants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Applications_JobPostings_JobPostingId",
                        column: x => x.JobPostingId,
                        principalTable: "JobPostings",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Applications_RecruitmentRequests_RecruitmentRequestId",
                        column: x => x.RecruitmentRequestId,
                        principalTable: "RecruitmentRequests",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ApplicationEvaluations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InterviewScheduleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EvaluatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false),
                    StrengthNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    WeaknessNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Recommendation = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationEvaluations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationEvaluations_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationEvaluations_InterviewSchedules_InterviewScheduleId",
                        column: x => x.InterviewScheduleId,
                        principalTable: "InterviewSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ApplicationEvaluations_Users_EvaluatorId",
                        column: x => x.EvaluatorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationStageHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FromStage = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ToStage = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ChangedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ChangedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationStageHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationStageHistories_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationStageHistories_Users_ChangedByUserId",
                        column: x => x.ChangedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobPostings_RecruitmentRequestId_Status",
                table: "JobPostings",
                columns: new[] { "RecruitmentRequestId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicantDocuments_ApplicantId",
                table: "ApplicantDocuments",
                column: "ApplicantId");

            migrationBuilder.CreateIndex(
                name: "IX_Applicants_Email",
                table: "Applicants",
                column: "Email",
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Applicants_Phone",
                table: "Applicants",
                column: "Phone",
                filter: "[Phone] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationEvaluations_ApplicationId_EvaluatorId_InterviewScheduleId",
                table: "ApplicationEvaluations",
                columns: new[] { "ApplicationId", "EvaluatorId", "InterviewScheduleId" },
                unique: true,
                filter: "[InterviewScheduleId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationEvaluations_EvaluatorId",
                table: "ApplicationEvaluations",
                column: "EvaluatorId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationEvaluations_InterviewScheduleId",
                table: "ApplicationEvaluations",
                column: "InterviewScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_ApplicantId",
                table: "Applications",
                column: "ApplicantId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_JobPostingId",
                table: "Applications",
                column: "JobPostingId",
                filter: "[JobPostingId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_RecruitmentRequestId_Stage",
                table: "Applications",
                columns: new[] { "RecruitmentRequestId", "Stage" });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationStageHistories_ApplicationId",
                table: "ApplicationStageHistories",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationStageHistories_ChangedByUserId",
                table: "ApplicationStageHistories",
                column: "ChangedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewRuleConfigSteps_InterviewRuleConfigId_RoundNumber",
                table: "InterviewRuleConfigSteps",
                columns: new[] { "InterviewRuleConfigId", "RoundNumber" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_InterviewSchedules_Applications_ApplicationId",
                table: "InterviewSchedules",
                column: "ApplicationId",
                principalTable: "Applications",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InterviewSchedules_Applications_ApplicationId",
                table: "InterviewSchedules");

            migrationBuilder.DropTable(
                name: "ApplicantDocuments");

            migrationBuilder.DropTable(
                name: "ApplicationEvaluations");

            migrationBuilder.DropTable(
                name: "ApplicationStageHistories");

            migrationBuilder.DropTable(
                name: "InterviewRuleConfigSteps");

            migrationBuilder.DropTable(
                name: "Applications");

            migrationBuilder.DropTable(
                name: "Applicants");

            migrationBuilder.DropIndex(
                name: "IX_JobPostings_RecruitmentRequestId_Status",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "JobType",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "Requirements",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "SalaryMax",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "SalaryMin",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "SalaryVisible",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "WorkingLocation",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "Round",
                table: "InterviewSchedules");

            migrationBuilder.RenameColumn(
                name: "ApplicationId",
                table: "InterviewSchedules",
                newName: "CandidateId");

            migrationBuilder.RenameIndex(
                name: "IX_InterviewSchedules_ApplicationId",
                table: "InterviewSchedules",
                newName: "IX_InterviewSchedules_CandidateId");

            migrationBuilder.AlterColumn<string>(
                name: "NotifyRoleKey",
                table: "InterviewRuleConfigs",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InterviewerRoleKey",
                table: "InterviewRuleConfigs",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "InterviewRuleConfigs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SchedulerRoleKey",
                table: "InterviewRuleConfigs",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Candidates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RecruitmentRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ConvertedEmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CvUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    SourceChannel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Stage = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TrialStartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Candidates_RecruitmentRequests_RecruitmentRequestId",
                        column: x => x.RecruitmentRequestId,
                        principalTable: "RecruitmentRequests",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RecruitmentApproverConfigs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApproverId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecruitmentApproverConfigs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecruitmentApproverConfigs_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RecruitmentApproverConfigs_Users_ApproverId",
                        column: x => x.ApproverId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CandidateEvaluations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CandidateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EvaluatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsStoreEvaluation = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Recommendation = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false),
                    StrengthNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WeaknessNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidateEvaluations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CandidateEvaluations_Candidates_CandidateId",
                        column: x => x.CandidateId,
                        principalTable: "Candidates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CandidateEvaluations_Users_EvaluatorId",
                        column: x => x.EvaluatorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobPostings_RecruitmentRequestId",
                table: "JobPostings",
                column: "RecruitmentRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateEvaluations_CandidateId_EvaluatorId",
                table: "CandidateEvaluations",
                columns: new[] { "CandidateId", "EvaluatorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CandidateEvaluations_EvaluatorId",
                table: "CandidateEvaluations",
                column: "EvaluatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Candidates_RecruitmentRequestId_Stage",
                table: "Candidates",
                columns: new[] { "RecruitmentRequestId", "Stage" });

            migrationBuilder.CreateIndex(
                name: "IX_RecruitmentApproverConfigs_ApproverId",
                table: "RecruitmentApproverConfigs",
                column: "ApproverId");

            migrationBuilder.CreateIndex(
                name: "IX_RecruitmentApproverConfigs_DepartmentId",
                table: "RecruitmentApproverConfigs",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_InterviewSchedules_Candidates_CandidateId",
                table: "InterviewSchedules",
                column: "CandidateId",
                principalTable: "Candidates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
