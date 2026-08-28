namespace Application;

public sealed record GetInterviewRuleConfigsQuery(
    RecruitmentRequestContext? Context,
    bool? IsActive
) : IRequest<IEnumerable<InterviewRuleConfigResponse>>;
