using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class HRM_AddRecruitmentPipeline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RecruitmentPipelines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecruitmentPipelines", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RecruitmentPipelineStages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PipelineId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoundTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsFixed = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecruitmentPipelineStages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecruitmentPipelineStages_RecruitmentPipelines_PipelineId",
                        column: x => x.PipelineId,
                        principalTable: "RecruitmentPipelines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecruitmentPipelineStages_RoundTypes_RoundTypeId",
                        column: x => x.RoundTypeId,
                        principalTable: "RoundTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecruitmentPipelineStages_PipelineId",
                table: "RecruitmentPipelineStages",
                column: "PipelineId");

            migrationBuilder.CreateIndex(
                name: "IX_RecruitmentPipelineStages_RoundTypeId",
                table: "RecruitmentPipelineStages",
                column: "RoundTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecruitmentPipelineStages");

            migrationBuilder.DropTable(
                name: "RecruitmentPipelines");
        }
    }
}
