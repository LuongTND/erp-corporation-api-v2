namespace Application;

public sealed record RejectCandidateCommand(Guid CandidateId, string RejectionReason) : IRequest<Unit>;
