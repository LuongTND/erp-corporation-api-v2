using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class HrmDropUnusedSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecruitmentRequests_Users_Level1ApproverId",
                table: "RecruitmentRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_RecruitmentRequests_Users_Level2ApproverId",
                table: "RecruitmentRequests");

            migrationBuilder.DropTable(
                name: "ConversationActivityLogs");

            migrationBuilder.DropTable(
                name: "ConversationMembers");

            migrationBuilder.DropTable(
                name: "MessageAttachments");

            migrationBuilder.DropTable(
                name: "MessageReactions");

            migrationBuilder.DropTable(
                name: "MessageReadStatuses");

            migrationBuilder.DropTable(
                name: "MessageTasks");

            migrationBuilder.DropTable(
                name: "TaskActivityLogs");

            migrationBuilder.DropTable(
                name: "TaskAssignees");

            migrationBuilder.DropTable(
                name: "TaskAttachments");

            migrationBuilder.DropTable(
                name: "TaskComments");

            migrationBuilder.DropTable(
                name: "TaskDependencies");

            migrationBuilder.DropTable(
                name: "TaskFollowers");

            migrationBuilder.DropTable(
                name: "TaskKpis");

            migrationBuilder.DropTable(
                name: "TaskLmsCourses");

            migrationBuilder.DropTable(
                name: "TaskTemplates");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "TaskItems");

            migrationBuilder.DropTable(
                name: "Conversations");

            migrationBuilder.DropTable(
                name: "TaskPriorities");

            migrationBuilder.DropTable(
                name: "TaskStatuses");

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
                name: "Level1Note",
                table: "RecruitmentRequests");

            migrationBuilder.DropColumn(
                name: "Level2ApprovedAt",
                table: "RecruitmentRequests");

            migrationBuilder.DropColumn(
                name: "Level2ApproverId",
                table: "RecruitmentRequests");

            migrationBuilder.DropColumn(
                name: "Level2Note",
                table: "RecruitmentRequests");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<string>(
                name: "Level1Note",
                table: "RecruitmentRequests",
                type: "nvarchar(1000)",
                maxLength: 1000,
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

            migrationBuilder.AddColumn<string>(
                name: "Level2Note",
                table: "RecruitmentRequests",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Conversations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConversationType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsArchived = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsPrivate = table.Column<bool>(type: "bit", nullable: false),
                    LastMessageAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaskPriorities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Color = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsSystem = table.Column<bool>(type: "bit", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskPriorities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaskStatuses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Color = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsFinalState = table.Column<bool>(type: "bit", nullable: false),
                    IsInitialState = table.Column<bool>(type: "bit", nullable: false),
                    IsSystem = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConversationActivityLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConversationID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConversationActivityLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConversationActivityLogs_Conversations_ConversationID",
                        column: x => x.ConversationID,
                        principalTable: "Conversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConversationActivityLogs_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConversationID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentMessageID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsEdited = table.Column<bool>(type: "bit", nullable: false),
                    MessageType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_Conversations_ConversationID",
                        column: x => x.ConversationID,
                        principalTable: "Conversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Messages_Messages_ParentMessageID",
                        column: x => x.ParentMessageID,
                        principalTable: "Messages",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Messages_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TaskTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DefaultPriorityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DefaultDurationDays = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    TemplateName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskTemplates_TaskPriorities_DefaultPriorityId",
                        column: x => x.DefaultPriorityId,
                        principalTable: "TaskPriorities",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TaskItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentTaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PriorityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActualHours = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    CompletedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    DueDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    EstimatedHours = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Progress = table.Column<int>(type: "int", nullable: false),
                    RecurringPattern = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    StartDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    TaskCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TaskType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskItems_TaskItems_ParentTaskId",
                        column: x => x.ParentTaskId,
                        principalTable: "TaskItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TaskItems_TaskPriorities_PriorityId",
                        column: x => x.PriorityId,
                        principalTable: "TaskPriorities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaskItems_TaskStatuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "TaskStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ConversationMembers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConversationID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastReadMessageID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsMuted = table.Column<bool>(type: "bit", nullable: false),
                    JoinedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LeftAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RoleInConversation = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConversationMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConversationMembers_Conversations_ConversationID",
                        column: x => x.ConversationID,
                        principalTable: "Conversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConversationMembers_Messages_LastReadMessageID",
                        column: x => x.LastReadMessageID,
                        principalTable: "Messages",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ConversationMembers_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MessageAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MessageID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FileSize = table.Column<int>(type: "int", nullable: false),
                    FileType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FileURL = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ThumbnailURL = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageAttachments_Messages_MessageID",
                        column: x => x.MessageID,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MessageReactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MessageID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ReactionType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageReactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageReactions_Messages_MessageID",
                        column: x => x.MessageID,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MessageReactions_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MessageReadStatuses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MessageID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ReadAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageReadStatuses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageReadStatuses_Messages_MessageID",
                        column: x => x.MessageID,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MessageReadStatuses_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MessageTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MessageID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaskID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LinkedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageTasks_Messages_MessageID",
                        column: x => x.MessageID,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MessageTasks_TaskItems_TaskID",
                        column: x => x.TaskID,
                        principalTable: "TaskItems",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TaskActivityLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaskID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    NewValue = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    OldValue = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskActivityLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskActivityLogs_TaskItems_TaskID",
                        column: x => x.TaskID,
                        principalTable: "TaskItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskActivityLogs_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TaskAssignees",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaskID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    AssignedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IsPrimaryAssignee = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskAssignees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskAssignees_TaskItems_TaskID",
                        column: x => x.TaskID,
                        principalTable: "TaskItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskAssignees_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TaskAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DataUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MimeType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Size = table.Column<long>(type: "bigint", nullable: false),
                    UploadedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskAttachments_TaskItems_TaskId",
                        column: x => x.TaskId,
                        principalTable: "TaskItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskComments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentCommentID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TaskID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskComments_TaskComments_ParentCommentID",
                        column: x => x.ParentCommentID,
                        principalTable: "TaskComments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TaskComments_TaskItems_TaskID",
                        column: x => x.TaskID,
                        principalTable: "TaskItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskComments_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TaskDependencies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BlockedTaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BlockerTaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DependencyType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskDependencies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskDependencies_TaskItems_BlockedTaskId",
                        column: x => x.BlockedTaskId,
                        principalTable: "TaskItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TaskDependencies_TaskItems_BlockerTaskId",
                        column: x => x.BlockerTaskId,
                        principalTable: "TaskItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskFollowers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaskID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    FollowedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskFollowers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskFollowers_TaskItems_TaskID",
                        column: x => x.TaskID,
                        principalTable: "TaskItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskFollowers_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TaskKpis",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaskID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    KpiId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Weight = table.Column<decimal>(type: "decimal(5,4)", precision: 5, scale: 4, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskKpis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskKpis_TaskItems_TaskID",
                        column: x => x.TaskID,
                        principalTable: "TaskItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskLmsCourses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaskID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CompletionStatus = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RequiredForCompletion = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskLmsCourses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskLmsCourses_TaskItems_TaskID",
                        column: x => x.TaskID,
                        principalTable: "TaskItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "IX_ConversationActivityLogs_ConversationID",
                table: "ConversationActivityLogs",
                column: "ConversationID");

            migrationBuilder.CreateIndex(
                name: "IX_ConversationActivityLogs_UserID",
                table: "ConversationActivityLogs",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_ConversationMembers_ConversationID_UserID",
                table: "ConversationMembers",
                columns: new[] { "ConversationID", "UserID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConversationMembers_LastReadMessageID",
                table: "ConversationMembers",
                column: "LastReadMessageID");

            migrationBuilder.CreateIndex(
                name: "IX_ConversationMembers_UserID",
                table: "ConversationMembers",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_MessageAttachments_MessageID",
                table: "MessageAttachments",
                column: "MessageID");

            migrationBuilder.CreateIndex(
                name: "IX_MessageReactions_MessageID_UserID_ReactionType",
                table: "MessageReactions",
                columns: new[] { "MessageID", "UserID", "ReactionType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MessageReactions_UserID",
                table: "MessageReactions",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_MessageReadStatuses_MessageID_UserID",
                table: "MessageReadStatuses",
                columns: new[] { "MessageID", "UserID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MessageReadStatuses_UserID",
                table: "MessageReadStatuses",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ConversationID",
                table: "Messages",
                column: "ConversationID");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ParentMessageID",
                table: "Messages",
                column: "ParentMessageID");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_UserID",
                table: "Messages",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_MessageTasks_MessageID_TaskID",
                table: "MessageTasks",
                columns: new[] { "MessageID", "TaskID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MessageTasks_TaskID",
                table: "MessageTasks",
                column: "TaskID");

            migrationBuilder.CreateIndex(
                name: "IX_TaskActivityLogs_TaskID",
                table: "TaskActivityLogs",
                column: "TaskID");

            migrationBuilder.CreateIndex(
                name: "IX_TaskActivityLogs_UserID",
                table: "TaskActivityLogs",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_TaskAssignees_TaskID_UserID",
                table: "TaskAssignees",
                columns: new[] { "TaskID", "UserID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskAssignees_UserID",
                table: "TaskAssignees",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_TaskAttachments_TaskId",
                table: "TaskAttachments",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskComments_ParentCommentID",
                table: "TaskComments",
                column: "ParentCommentID");

            migrationBuilder.CreateIndex(
                name: "IX_TaskComments_TaskID",
                table: "TaskComments",
                column: "TaskID");

            migrationBuilder.CreateIndex(
                name: "IX_TaskComments_UserID",
                table: "TaskComments",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_TaskDependencies_BlockedTaskId",
                table: "TaskDependencies",
                column: "BlockedTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskDependencies_BlockerTaskId_BlockedTaskId",
                table: "TaskDependencies",
                columns: new[] { "BlockerTaskId", "BlockedTaskId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskFollowers_TaskID_UserID",
                table: "TaskFollowers",
                columns: new[] { "TaskID", "UserID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskFollowers_UserID",
                table: "TaskFollowers",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_TaskItems_ParentTaskId",
                table: "TaskItems",
                column: "ParentTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskItems_PriorityId",
                table: "TaskItems",
                column: "PriorityId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskItems_StatusId",
                table: "TaskItems",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskItems_TaskCode",
                table: "TaskItems",
                column: "TaskCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskKpis_TaskID_KpiId",
                table: "TaskKpis",
                columns: new[] { "TaskID", "KpiId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskLmsCourses_TaskID_CourseId",
                table: "TaskLmsCourses",
                columns: new[] { "TaskID", "CourseId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskPriorities_Code",
                table: "TaskPriorities",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskStatuses_Code",
                table: "TaskStatuses",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskTemplates_DefaultPriorityId",
                table: "TaskTemplates",
                column: "DefaultPriorityId");

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
    }
}
