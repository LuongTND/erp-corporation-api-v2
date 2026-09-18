using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations;

public partial class HrmCollapsePendingApprovalStatus : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // PendingLevel1Approval=2, PendingLevel2Approval=3 → PendingApproval=2
        migrationBuilder.Sql(
            "UPDATE RecruitmentRequests SET Status = 2 WHERE Status = 3");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Không thể phân biệt lại L1/L2 sau khi đã gộp
    }
}
