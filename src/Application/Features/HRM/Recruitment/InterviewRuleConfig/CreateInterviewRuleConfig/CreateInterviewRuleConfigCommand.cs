namespace Application;

public sealed record CreateInterviewRuleConfigCommand(
    string Name,
    RecruitmentRequestContext Context,
    Guid? RegionId,
    Guid? DepartmentId,
    string InterviewerRoleKey,
    InterviewLocation Location,
    string SchedulerRoleKey,
    string NotifyRoleKey,
    int Priority
) : IRequest<Guid>;
