namespace Application;

public sealed record UpdateInterviewRuleConfigCommand(
    Guid Id,
    string Name,
    string? NotifyRoleKey,
    int Priority,
    bool IsActive,
    IEnumerable<CreateInterviewRuleConfigStepCommand> Steps
) : IRequest<Unit>;
