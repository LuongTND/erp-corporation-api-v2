namespace Application;

public sealed record GetCandidateDetailQuery(Guid CandidateId) : IRequest<CandidateDetailResponse>;
