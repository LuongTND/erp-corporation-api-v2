namespace Application;

public sealed record ApproveLevel1RecruitmentRequestCommand(Guid RequestId) : IRequest<Unit>;
