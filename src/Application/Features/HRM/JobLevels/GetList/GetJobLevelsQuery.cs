namespace Application;

public sealed record GetJobLevelsQuery(QueryInfo QueryInfo) : IRequest<QueryResult<JobLevelResponse>>;
