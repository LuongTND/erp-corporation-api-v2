namespace Application;

public sealed record ResolveInterviewRuleQuery(Guid ApplicationId) : IRequest<InterviewRuleConfigResponse?>;
