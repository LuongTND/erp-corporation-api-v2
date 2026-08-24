namespace Application;

public sealed record GetRecruitmentApproversQuery : IRequest<IEnumerable<RecruitmentApproverConfigResponse>>;
