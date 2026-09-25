namespace Application;

public sealed record RejectRecruitmentRequestCommand(
    Guid RequestId,
    string RejectionNote
) : IRequest<Unit>;
