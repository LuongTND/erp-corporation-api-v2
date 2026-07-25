using Role = Domain.Role;
using TaskItemStatus = Domain.TaskItemStatus;

namespace Infrastructure;

public partial class ApplicationDbContext
{
    // Audit
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    // Outbox
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    // System
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    // Users
    public DbSet<User> Users => Set<User>();
    public DbSet<UserAccount> UserAccounts => Set<UserAccount>();
    public DbSet<UserDepartment> UserDepartments => Set<UserDepartment>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();

    // Org
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<JobLevel> JobLevels => Set<JobLevel>();

    // Tasks
    public DbSet<TaskItemStatus> TaskStatuses => Set<TaskItemStatus>();
    public DbSet<TaskPriority> TaskPriorities => Set<TaskPriority>();
    public DbSet<TaskItem> TaskItems => Set<TaskItem>();
    public DbSet<TaskAssignee> TaskAssignees => Set<TaskAssignee>();
    public DbSet<TaskFollower> TaskFollowers => Set<TaskFollower>();
    public DbSet<TaskComment> TaskComments => Set<TaskComment>();
    public DbSet<TaskAttachment> TaskAttachments => Set<TaskAttachment>();
    public DbSet<TaskActivityLog> TaskActivityLogs => Set<TaskActivityLog>();
    public DbSet<TaskKpi> TaskKpis => Set<TaskKpi>();
    public DbSet<TaskLmsCourse> TaskLmsCourses => Set<TaskLmsCourse>();
    public DbSet<TaskTemplate> TaskTemplates => Set<TaskTemplate>();
    public DbSet<TaskDependency> TaskDependencies => Set<TaskDependency>();

    // Chat
    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<ConversationMember> ConversationMembers => Set<ConversationMember>();
    public DbSet<ConversationActivityLog> ConversationActivityLogs => Set<ConversationActivityLog>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<MessageAttachment> MessageAttachments => Set<MessageAttachment>();
    public DbSet<MessageReaction> MessageReactions => Set<MessageReaction>();
    public DbSet<MessageReadStatus> MessageReadStatuses => Set<MessageReadStatus>();
    public DbSet<MessageTask> MessageTasks => Set<MessageTask>();

    // Notifications
    public DbSet<NotificationEventType> NotificationEventTypes => Set<NotificationEventType>();
    public DbSet<NotificationTemplate> NotificationTemplates => Set<NotificationTemplate>();
    public DbSet<NotificationTriggerBinding> NotificationTriggerBindings => Set<NotificationTriggerBinding>();
    public DbSet<UserNotification> UserNotifications => Set<UserNotification>();

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entities = ChangeTracker
            .Entries<EntityBase<Guid>>()
            .Where(e => e.Entity.DomainEvents.Count > 0)
            .Select(e => e.Entity)
            .ToList();

        var events = entities.SelectMany(e => e.DomainEvents).ToList();
        entities.ForEach(e => e.ClearDomainEvents());

        foreach (var domainEvent in events)
        {
            var (type, payload) = OutboxSerializer.Serialize(domainEvent);
            OutboxMessages.Add(new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Type = type,
                Payload = payload,
                OccurredOn = domainEvent.OccurredOn,
                CreatedAt = DateTimeOffset.UtcNow
            });
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
