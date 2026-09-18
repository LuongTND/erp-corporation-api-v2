namespace Application;

public sealed record CreateInterviewRuleConfigStepCommand(
    int RoundNumber,
    string Label,
    string InterviewerRoleKey,
    string SchedulerRoleKey,
    InterviewLocation Location
);

public sealed record CreateInterviewRuleConfigCommand(
    string Name,
    RecruitmentRequestContext Context,
    Guid? RegionId,
    Guid? DepartmentId,
    string? NotifyRoleKey,
    int Priority,
    IEnumerable<CreateInterviewRuleConfigStepCommand> Steps
) : IRequest<Guid>;
