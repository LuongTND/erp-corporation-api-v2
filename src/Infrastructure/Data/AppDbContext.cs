namespace Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<JobLevel> JobLevels => Set<JobLevel>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserAccount> UserAccounts => Set<UserAccount>();
    public DbSet<UserDepartment> UserDepartments => Set<UserDepartment>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<NotificationEventType> NotificationEventTypes => Set<NotificationEventType>();
    public DbSet<NotificationTemplate> NotificationTemplates => Set<NotificationTemplate>();
    public DbSet<NotificationTriggerBinding> NotificationTriggerBindings => Set<NotificationTriggerBinding>();
    public DbSet<UserNotification> UserNotifications => Set<UserNotification>();

    // Phân hệ Quản lý công việc (Task)
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<TaskAssignee> TaskAssignees => Set<TaskAssignee>();
    public DbSet<TaskFollower> TaskFollowers => Set<TaskFollower>();
    public DbSet<TaskComment> TaskComments => Set<TaskComment>();
    public DbSet<TaskActivityLog> TaskActivityLogs => Set<TaskActivityLog>();
    public DbSet<TaskKpi> TaskKpis => Set<TaskKpi>();
    public DbSet<TaskLmsCourse> TaskLmsCourses => Set<TaskLmsCourse>();
    public DbSet<TaskTemplate> TaskTemplates => Set<TaskTemplate>();

    // Phân hệ Chat nội bộ (Chat)
    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<ConversationMember> ConversationMembers => Set<ConversationMember>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<MessageAttachment> MessageAttachments => Set<MessageAttachment>();
    public DbSet<MessageReaction> MessageReactions => Set<MessageReaction>();
    public DbSet<MessageReadStatus> MessageReadStatuses => Set<MessageReadStatus>();
    public DbSet<MessageTask> MessageTasks => Set<MessageTask>();
    public DbSet<ConversationActivityLog> ConversationActivityLogs => Set<ConversationActivityLog>();

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var domainEvents = ChangeTracker
            .Entries<BaseEntity>()
            .Where(e => e.Entity.DomainEvents.Count > 0)
            .Select(e => e.Entity)
            .ToList();

        var events = domainEvents
            .SelectMany(e => e.DomainEvents)
            .ToList();

        domainEvents.ForEach(e => e.ClearDomainEvents());

        foreach (var domainEvent in events)
        {
            var (type, payload) = OutboxSerializer.Serialize(domainEvent);
            OutboxMessages.Add(new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Type = type,
                Payload = payload,
                OccurredOn = domainEvent.OccurredOn,
                CreatedAt = DateTime.UtcNow
            });
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(AssemblyReference.Assembly);
        base.OnModelCreating(modelBuilder);
    }
}