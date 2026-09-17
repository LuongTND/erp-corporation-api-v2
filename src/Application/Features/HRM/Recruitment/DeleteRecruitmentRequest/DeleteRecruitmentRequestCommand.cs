namespace Application;

public sealed record DeleteRecruitmentRequestCommand(Guid RequestId, string? Note) : IRequest<Unit>;
