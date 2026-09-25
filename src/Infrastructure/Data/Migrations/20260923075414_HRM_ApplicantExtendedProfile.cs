using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class HRM_ApplicantExtendedProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Applicants_Email",
                table: "Applicants");

            migrationBuilder.DropIndex(
                name: "IX_Applicants_Phone",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Applicants");

            migrationBuilder.RenameColumn(
                name: "EducationLevel",
                table: "Applicants",
                newName: "Gender");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Applicants",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "DateOfBirth",
                table: "Applicants",
                type: "date",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DocumentType",
                table: "ApplicantDocuments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.CreateTable(
                name: "ApplicantEducations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EducationLevel = table.Column<int>(type: "int", nullable: false),
                    SchoolName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Major = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    GraduationYear = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicantEducations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicantEducations_Applicants_ApplicantId",
                        column: x => x.ApplicantId,
                        principalTable: "Applicants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApplicantEmails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicantEmails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicantEmails_Applicants_ApplicantId",
                        column: x => x.ApplicantId,
                        principalTable: "Applicants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApplicantExperiences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RecentWorkplace = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CompanyName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Position = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicantExperiences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicantExperiences_Applicants_ApplicantId",
                        column: x => x.ApplicantId,
                        principalTable: "Applicants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApplicantPhones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicantPhones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicantPhones_Applicants_ApplicantId",
                        column: x => x.ApplicantId,
                        principalTable: "Applicants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicantEducations_ApplicantId",
                table: "ApplicantEducations",
                column: "ApplicantId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicantEmails_ApplicantId",
                table: "ApplicantEmails",
                column: "ApplicantId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicantExperiences_ApplicantId",
                table: "ApplicantExperiences",
                column: "ApplicantId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicantPhones_ApplicantId",
                table: "ApplicantPhones",
                column: "ApplicantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicantEducations");

            migrationBuilder.DropTable(
                name: "ApplicantEmails");

            migrationBuilder.DropTable(
                name: "ApplicantExperiences");

            migrationBuilder.DropTable(
                name: "ApplicantPhones");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "Applicants");

            migrationBuilder.RenameColumn(
                name: "Gender",
                table: "Applicants",
                newName: "EducationLevel");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Applicants",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Applicants",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DocumentType",
                table: "ApplicantDocuments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

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
        }
    }
}
