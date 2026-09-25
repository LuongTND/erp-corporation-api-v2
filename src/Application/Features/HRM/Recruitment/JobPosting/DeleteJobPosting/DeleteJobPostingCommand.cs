namespace Application;

public sealed record DeleteJobPostingCommand(Guid PostingId) : IRequest<Unit>;
