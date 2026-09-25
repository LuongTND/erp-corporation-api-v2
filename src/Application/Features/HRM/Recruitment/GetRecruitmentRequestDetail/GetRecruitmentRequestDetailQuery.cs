namespace Application;

public sealed record GetRecruitmentRequestDetailQuery(Guid RequestId)
    : IRequest<RecruitmentRequestDetailResponse>;
