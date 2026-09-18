namespace Application;

public sealed record GetCandidateDetailQuery(Guid ApplicationId) : IRequest<CandidateDetailResponse>;
