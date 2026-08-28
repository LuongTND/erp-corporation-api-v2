namespace Application;

public sealed record SubmitRecruitmentRequestCommand(Guid RequestId) : IRequest<Unit>;
