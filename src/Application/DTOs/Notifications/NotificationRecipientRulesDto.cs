namespace Application.DTOs.Notifications;

public record NotificationRecipientRulesDto
{
    public bool IncludeSubjectUser { get; init; }
    public bool IncludeActor { get; init; }
    public bool IncludeSuperAdmins { get; init; }
    public bool IncludeDepartmentManager { get; init; }
    public List<Guid> RoleIds { get; init; } = [];
    public List<Guid> UserIds { get; init; } = [];
}

public record NotificationPublishContext
{
    public Guid? SubjectUserId { get; init; }
    public Guid? ActorUserId { get; init; }
}
