namespace Application;

public sealed record ApproveRecruitmentRequestCommand(Guid RequestId, string? Note) : IRequest<Unit>;
