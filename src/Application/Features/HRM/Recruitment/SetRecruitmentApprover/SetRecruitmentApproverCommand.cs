namespace Application;

public sealed record SetRecruitmentApproverCommand(
    Guid    ApproverId,
    Guid?   DepartmentId,
    string? Note
) : IRequest<Guid>;
