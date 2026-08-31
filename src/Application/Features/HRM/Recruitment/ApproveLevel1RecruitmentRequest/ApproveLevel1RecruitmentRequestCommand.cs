namespace Application;

public sealed record ApproveLevel1RecruitmentRequestCommand(Guid RequestId, string? Note) : IRequest<Unit>;
