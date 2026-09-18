namespace Application;

public sealed record DeleteJobTitleCommand(Guid JobTitleId) : IRequest<Unit>;
