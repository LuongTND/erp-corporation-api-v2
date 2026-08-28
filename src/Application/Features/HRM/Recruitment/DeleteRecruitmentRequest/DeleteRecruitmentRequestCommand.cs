namespace Application;

public sealed record DeleteRecruitmentRequestCommand(Guid RequestId) : IRequest<Unit>;
