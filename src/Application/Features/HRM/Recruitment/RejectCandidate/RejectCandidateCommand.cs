namespace Application;

public sealed record RejectCandidateCommand(Guid ApplicationId, string RejectionReason) : IRequest<Unit>;
