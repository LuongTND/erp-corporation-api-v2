namespace Application;

public sealed record RejectJobPostingCostCommand(Guid PostingId, string RejectionNote) : IRequest<Unit>;
