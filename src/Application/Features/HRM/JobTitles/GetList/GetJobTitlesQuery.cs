namespace Application;

public sealed record GetJobTitlesQuery(QueryInfo QueryInfo) : IRequest<QueryResult<JobTitleResponse>>;
