using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkflowTaskRoleAssignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "AssignedTo",
                table: "WorkflowTasks",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "AssignedToRoleId",
                table: "WorkflowTasks",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "WorkflowTasks",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowTasks_AssignedToRoleId_Status",
                table: "WorkflowTasks",
                columns: new[] { "AssignedToRoleId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WorkflowTasks_AssignedToRoleId_Status",
                table: "WorkflowTasks");

            migrationBuilder.DropColumn(
                name: "AssignedToRoleId",
                table: "WorkflowTasks");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "WorkflowTasks");

            migrationBuilder.AlterColumn<Guid>(
                name: "AssignedTo",
                table: "WorkflowTasks",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}
