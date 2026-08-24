namespace Application;

public sealed record DeleteRecruitmentApproverCommand(Guid ConfigId) : IRequest<Unit>;
