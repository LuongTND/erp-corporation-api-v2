namespace Application;

public sealed record UpdateRecruitmentRequestCommand(
    Guid RequestId,
    string PositionTitle,
    int Headcount,
    string Reason,
    string? JobDescription,
    DateOnly? RequiredByDate
) : IRequest<Unit>;
