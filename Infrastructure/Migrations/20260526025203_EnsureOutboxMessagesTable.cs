using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations;

/// <summary>
/// Khắc phục migration Up() rỗng trước đó — tạo bảng OutboxMessages nếu chưa có.
/// </summary>
public partial class EnsureOutboxMessagesTable : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            IF OBJECT_ID(N'dbo.OutboxMessages', N'U') IS NULL
            BEGIN
                CREATE TABLE dbo.OutboxMessages (
                    Id uniqueidentifier NOT NULL,
                    Type nvarchar(500) NOT NULL,
                    Payload nvarchar(max) NOT NULL,
                    OccurredOn datetime2 NOT NULL,
                    CreatedAt datetime2 NOT NULL,
                    ProcessedAt datetime2 NULL,
                    Error nvarchar(4000) NULL,
                    RetryCount int NOT NULL,
                    CONSTRAINT PK_OutboxMessages PRIMARY KEY (Id)
                );

                CREATE INDEX IX_OutboxMessages_ProcessedAt ON dbo.OutboxMessages (ProcessedAt);
                CREATE INDEX IX_OutboxMessages_ProcessedAt_CreatedAt ON dbo.OutboxMessages (ProcessedAt, CreatedAt);
            END
            """);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            IF OBJECT_ID(N'dbo.OutboxMessages', N'U') IS NOT NULL
                DROP TABLE dbo.OutboxMessages;
            """);
    }
}
