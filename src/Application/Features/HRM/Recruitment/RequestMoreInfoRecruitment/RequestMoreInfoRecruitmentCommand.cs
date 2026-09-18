namespace Application;

public sealed record RequestMoreInfoRecruitmentCommand(
    Guid RequestId,
    string NeedMoreInfoNote
) : IRequest<Unit>;
