namespace Application;

public sealed record ApproveRecruitmentRequestCommand(Guid RequestId) : IRequest<Unit>;
