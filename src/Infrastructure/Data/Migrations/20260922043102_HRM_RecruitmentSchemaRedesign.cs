using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class HRM_RecruitmentSchemaRedesign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InterviewSchedules_Candidates_CandidateId",
                table: "InterviewSchedules");

            migrationBuilder.DropForeignKey(
                name: "FK_InterviewSchedules_Users_InterviewerId",
                table: "InterviewSchedules");

            migrationBuilder.DropForeignKey(
                name: "FK_JobPostings_Users_CostApprovedByUserId",
                table: "JobPostings");

            migrationBuilder.DropTable(
                name: "CandidateEvaluations");

            migrationBuilder.DropTable(
                name: "InterviewRuleConfigs");

            migrationBuilder.DropTable(
                name: "RecruitmentApproverConfigs");

            migrationBuilder.DropTable(
                name: "Candidates");

            migrationBuilder.DropIndex(
                name: "IX_InterviewSchedules_InterviewerId_ScheduledAt",
                table: "InterviewSchedules");

            migrationBuilder.DropColumn(
                name: "CostApprovedAt",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "CostRejectionNote",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "CostStatus",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "EstimatedCost",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "InterviewSchedules");

            migrationBuilder.RenameColumn(
                name: "CostApprovedByUserId",
                table: "JobPostings",
                newName: "AssignedToUserId");

            migrationBuilder.RenameIndex(
                name: "IX_JobPostings_CostApprovedByUserId",
                table: "JobPostings",
                newName: "IX_JobPostings_AssignedToUserId");

            migrationBuilder.RenameColumn(
                name: "CandidateId",
                table: "InterviewSchedules",
                newName: "ApplicationId");

            migrationBuilder.RenameIndex(
                name: "IX_InterviewSchedules_CandidateId",
                table: "InterviewSchedules",
                newName: "IX_InterviewSchedules_ApplicationId");

            migrationBuilder.AddColumn<bool>(
                name: "InterviewAtStore",
                table: "Regions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "PostUrl",
                table: "JobPostings",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "ExpiresAt",
                table: "JobPostings",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Channel",
                table: "JobPostings",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "Requirements",
                table: "JobPostings",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "JobPostings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "InterviewSchedules",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<Guid>(
                name: "InterviewerId",
                table: "InterviewSchedules",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<string>(
                name: "InterviewerRoleKey",
                table: "Departments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NotifyAfterHiredRoleKey",
                table: "Departments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Applicants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    EducationLevel = table.Column<int>(type: "int", nullable: true),
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
                    JobPostingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceChannel = table.Column<int>(type: "int", nullable: false),
                    Stage = table.Column<int>(type: "int", nullable: false),
                    RejectionReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TrialStartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ConvertedEmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RecruitmentRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Applications_JobPostings_JobPostingId",
                        column: x => x.JobPostingId,
                        principalTable: "JobPostings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                name: "IX_InterviewSchedules_InterviewerId",
                table: "InterviewSchedules",
                column: "InterviewerId");

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
                column: "JobPostingId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_RecruitmentRequestId",
                table: "Applications",
                column: "RecruitmentRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationStageHistories_ApplicationId",
                table: "ApplicationStageHistories",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationStageHistories_ChangedByUserId",
                table: "ApplicationStageHistories",
                column: "ChangedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_InterviewSchedules_Applications_ApplicationId",
                table: "InterviewSchedules",
                column: "ApplicationId",
                principalTable: "Applications",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InterviewSchedules_Users_InterviewerId",
                table: "InterviewSchedules",
                column: "InterviewerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_JobPostings_Users_AssignedToUserId",
                table: "JobPostings",
                column: "AssignedToUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InterviewSchedules_Applications_ApplicationId",
                table: "InterviewSchedules");

            migrationBuilder.DropForeignKey(
                name: "FK_InterviewSchedules_Users_InterviewerId",
                table: "InterviewSchedules");

            migrationBuilder.DropForeignKey(
                name: "FK_JobPostings_Users_AssignedToUserId",
                table: "JobPostings");

            migrationBuilder.DropTable(
                name: "ApplicantDocuments");

            migrationBuilder.DropTable(
                name: "ApplicationEvaluations");

            migrationBuilder.DropTable(
                name: "ApplicationStageHistories");

            migrationBuilder.DropTable(
                name: "Applications");

            migrationBuilder.DropTable(
                name: "Applicants");

            migrationBuilder.DropIndex(
                name: "IX_InterviewSchedules_InterviewerId",
                table: "InterviewSchedules");

            migrationBuilder.DropColumn(
                name: "InterviewAtStore",
                table: "Regions");

            migrationBuilder.DropColumn(
                name: "Requirements",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "InterviewerRoleKey",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "NotifyAfterHiredRoleKey",
                table: "Departments");

            migrationBuilder.RenameColumn(
                name: "AssignedToUserId",
                table: "JobPostings",
                newName: "CostApprovedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_JobPostings_AssignedToUserId",
                table: "JobPostings",
                newName: "IX_JobPostings_CostApprovedByUserId");

            migrationBuilder.RenameColumn(
                name: "ApplicationId",
                table: "InterviewSchedules",
                newName: "CandidateId");

            migrationBuilder.RenameIndex(
                name: "IX_InterviewSchedules_ApplicationId",
                table: "InterviewSchedules",
                newName: "IX_InterviewSchedules_CandidateId");

            migrationBuilder.AlterColumn<string>(
                name: "PostUrl",
                table: "JobPostings",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "ExpiresAt",
                table: "JobPostings",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Channel",
                table: "JobPostings",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CostApprovedAt",
                table: "JobPostings",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CostRejectionNote",
                table: "JobPostings",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CostStatus",
                table: "JobPostings",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "EstimatedCost",
                table: "JobPostings",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "JobPostings",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "InterviewSchedules",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

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
                name: "Location",
                table: "InterviewSchedules",
                type: "nvarchar(50)",
                maxLength: 50,
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
                name: "InterviewRuleConfigs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RegionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Context = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InterviewerRoleKey = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Location = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NotifyRoleKey = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    SchedulerRoleKey = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
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
                name: "IX_InterviewSchedules_InterviewerId_ScheduledAt",
                table: "InterviewSchedules",
                columns: new[] { "InterviewerId", "ScheduledAt" });

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

            migrationBuilder.AddForeignKey(
                name: "FK_InterviewSchedules_Users_InterviewerId",
                table: "InterviewSchedules",
                column: "InterviewerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobPostings_Users_CostApprovedByUserId",
                table: "JobPostings",
                column: "CostApprovedByUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
