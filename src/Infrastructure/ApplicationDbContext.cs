using Role = Domain.Role;
using TaskItemStatus = Domain.TaskItemStatus;

namespace Infrastructure;

public sealed class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    // Audit
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<PermissionAuditLog> PermissionAuditLogs => Set<PermissionAuditLog>();

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
    public DbSet<EmployeeProfile> EmployeeProfiles => Set<EmployeeProfile>();
    public DbSet<EmployeeIdentity> EmployeeIdentities => Set<EmployeeIdentity>();
    public DbSet<EmploymentInfo> EmploymentInfos => Set<EmploymentInfo>();
    public DbSet<EmployeeDocument> EmployeeDocuments => Set<EmployeeDocument>();
    public DbSet<WorkHistory> WorkHistories => Set<WorkHistory>();

    // Labels
    public DbSet<Label> Labels => Set<Label>();
    public DbSet<UserLabel> UserLabels => Set<UserLabel>();

    // Recruitment
    public DbSet<RecruitmentApproverConfig> RecruitmentApproverConfigs => Set<RecruitmentApproverConfig>();
    public DbSet<RecruitmentRequest> RecruitmentRequests => Set<RecruitmentRequest>();
    public DbSet<Candidate> Candidates => Set<Candidate>();
    public DbSet<CandidateEvaluation> CandidateEvaluations => Set<CandidateEvaluation>();
    public DbSet<JobPosting> JobPostings => Set<JobPosting>();
    public DbSet<InterviewRuleConfig> InterviewRuleConfigs => Set<InterviewRuleConfig>();
    public DbSet<InterviewSchedule> InterviewSchedules => Set<InterviewSchedule>();

    // Custom Fields
    public DbSet<CustomFieldDefinition> CustomFieldDefinitions => Set<CustomFieldDefinition>();
    public DbSet<CustomFieldOption> CustomFieldOptions => Set<CustomFieldOption>();
    public DbSet<UserCustomFieldValue> UserCustomFieldValues => Set<UserCustomFieldValue>();

    // Org
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Region> Regions => Set<Region>();
    public DbSet<RegionHours> RegionHours => Set<RegionHours>();
    public DbSet<Store> Stores => Set<Store>();
    public DbSet<StoreHours> StoreHours => Set<StoreHours>();
    public DbSet<Counter> Counters => Set<Counter>();
    public DbSet<UserStore> UserStores => Set<UserStore>();
    public DbSet<JobLevel> JobLevels => Set<JobLevel>();
    public DbSet<EmployeeType> EmployeeTypes => Set<EmployeeType>();
    public DbSet<DepartmentJobLevel> DepartmentJobLevels => Set<DepartmentJobLevel>();
    public DbSet<BonusPolicy> BonusPolicies => Set<BonusPolicy>();

    // KPI
    public DbSet<KpiTemplate> KpiTemplates => Set<KpiTemplate>();
    public DbSet<KpiMetric> KpiMetrics => Set<KpiMetric>();
    public DbSet<KpiEntry> KpiEntries => Set<KpiEntry>();

    // Contracts
    public DbSet<ContractTemplate> ContractTemplates => Set<ContractTemplate>();
    public DbSet<EmploymentContract> EmploymentContracts => Set<EmploymentContract>();


    // Salary
    public DbSet<SalaryRecord> SalaryRecords => Set<SalaryRecord>();

    // Payroll
    public DbSet<PayrollRun> PayrollRuns => Set<PayrollRun>();
    public DbSet<PayrollEntry> PayrollEntries => Set<PayrollEntry>();

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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;
            if (typeof(ISoftDeletable).IsAssignableFrom(clrType))
                typeof(ApplicationDbContext)
                    .GetMethod(nameof(SetSoftDeleteFilter), BindingFlags.NonPublic | BindingFlags.Instance)!
                    .MakeGenericMethod(clrType)
                    .Invoke(this, [modelBuilder]);
        }
    }

    private void SetSoftDeleteFilter<TEntity>(ModelBuilder modelBuilder)
        where TEntity : class, ISoftDeletable
    {
        modelBuilder.Entity<TEntity>()
            .HasQueryFilter(e => !e.IsDeleted);
    }

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
