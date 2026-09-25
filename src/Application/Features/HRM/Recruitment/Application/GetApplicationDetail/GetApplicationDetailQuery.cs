namespace Application;

public sealed record GetApplicationDetailQuery(Guid ApplicationId) : IRequest<ApplicationDetailResponse>;
