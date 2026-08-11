using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class HRM025_DepartmentJobLevelAndKpi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentJobLevelId",
                table: "UserDepartments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BonusPolicies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BonusPolicies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KpiTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JobLevelId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KpiTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KpiTemplates_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_KpiTemplates_JobLevels_JobLevelId",
                        column: x => x.JobLevelId,
                        principalTable: "JobLevels",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DepartmentJobLevels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JobLevelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MinSalary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MaxSalary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    BonusPolicyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    KpiTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentJobLevels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DepartmentJobLevels_BonusPolicies_BonusPolicyId",
                        column: x => x.BonusPolicyId,
                        principalTable: "BonusPolicies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DepartmentJobLevels_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DepartmentJobLevels_JobLevels_JobLevelId",
                        column: x => x.JobLevelId,
                        principalTable: "JobLevels",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DepartmentJobLevels_KpiTemplates_KpiTemplateId",
                        column: x => x.KpiTemplateId,
                        principalTable: "KpiTemplates",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "KpiMetrics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    Target = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KpiMetrics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KpiMetrics_KpiTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "KpiTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserDepartments_DepartmentJobLevelId",
                table: "UserDepartments",
                column: "DepartmentJobLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentJobLevels_BonusPolicyId",
                table: "DepartmentJobLevels",
                column: "BonusPolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentJobLevels_DepartmentId_JobLevelId",
                table: "DepartmentJobLevels",
                columns: new[] { "DepartmentId", "JobLevelId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentJobLevels_JobLevelId",
                table: "DepartmentJobLevels",
                column: "JobLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentJobLevels_KpiTemplateId",
                table: "DepartmentJobLevels",
                column: "KpiTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_KpiMetrics_TemplateId",
                table: "KpiMetrics",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_KpiTemplates_DepartmentId_JobLevelId",
                table: "KpiTemplates",
                columns: new[] { "DepartmentId", "JobLevelId" },
                unique: true,
                filter: "[JobLevelId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_KpiTemplates_JobLevelId",
                table: "KpiTemplates",
                column: "JobLevelId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserDepartments_DepartmentJobLevels_DepartmentJobLevelId",
                table: "UserDepartments",
                column: "DepartmentJobLevelId",
                principalTable: "DepartmentJobLevels",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserDepartments_DepartmentJobLevels_DepartmentJobLevelId",
                table: "UserDepartments");

            migrationBuilder.DropTable(
                name: "DepartmentJobLevels");

            migrationBuilder.DropTable(
                name: "KpiMetrics");

            migrationBuilder.DropTable(
                name: "BonusPolicies");

            migrationBuilder.DropTable(
                name: "KpiTemplates");

            migrationBuilder.DropIndex(
                name: "IX_UserDepartments_DepartmentJobLevelId",
                table: "UserDepartments");

            migrationBuilder.DropColumn(
                name: "DepartmentJobLevelId",
                table: "UserDepartments");
        }
    }
}
