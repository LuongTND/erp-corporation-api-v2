namespace Application;

public sealed record ApproveJobPostingCostCommand(Guid PostingId) : IRequest<Unit>;
