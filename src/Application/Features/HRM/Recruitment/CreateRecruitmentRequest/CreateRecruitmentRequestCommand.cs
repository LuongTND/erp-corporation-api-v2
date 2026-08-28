namespace Application;

public sealed record CreateRecruitmentRequestCommand(
    RecruitmentRequestContext RequestContext,
    Guid? DepartmentId,
    Guid? StoreId,
    string PositionTitle,
    int Headcount,
    string Reason,
    string? JobDescription,
    DateOnly? RequiredByDate
) : IRequest<Guid>;
