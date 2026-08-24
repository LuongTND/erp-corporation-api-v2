using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class HRM_AddRegionManagerId_DropUserStoreUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserStores_UserId_StoreId",
                table: "UserStores");

            migrationBuilder.CreateIndex(
                name: "IX_UserStores_UserId_StoreId",
                table: "UserStores",
                columns: new[] { "UserId", "StoreId" });

            migrationBuilder.AddColumn<Guid>(
                name: "ManagerId",
                table: "Regions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Regions_ManagerId",
                table: "Regions",
                column: "ManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Regions_Users_ManagerId",
                table: "Regions",
                column: "ManagerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Regions_Users_ManagerId",
                table: "Regions");

            migrationBuilder.DropIndex(
                name: "IX_Regions_ManagerId",
                table: "Regions");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                table: "Regions");

            migrationBuilder.DropIndex(
                name: "IX_UserStores_UserId_StoreId",
                table: "UserStores");

            migrationBuilder.CreateIndex(
                name: "IX_UserStores_UserId_StoreId",
                table: "UserStores",
                columns: new[] { "UserId", "StoreId" },
                unique: true);
        }
    }
}
