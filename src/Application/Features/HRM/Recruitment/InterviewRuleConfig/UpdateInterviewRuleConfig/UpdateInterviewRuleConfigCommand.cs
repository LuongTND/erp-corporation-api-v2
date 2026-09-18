namespace Application;

public sealed record UpdateInterviewRuleConfigCommand(
    Guid Id,
    string Name,
    string InterviewerRoleKey,
    InterviewLocation Location,
    string SchedulerRoleKey,
    string NotifyRoleKey,
    int Priority,
    bool IsActive
) : IRequest<Unit>;
