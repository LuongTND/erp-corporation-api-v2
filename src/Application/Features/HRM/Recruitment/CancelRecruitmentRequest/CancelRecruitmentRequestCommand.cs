namespace Application;

public sealed record CancelRecruitmentRequestCommand(Guid RequestId, string? Note) : IRequest<Unit>;
