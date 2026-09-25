namespace Application;

public sealed record GetRecruitmentRequestsQuery(
    QueryInfo QueryInfo,
    RecruitmentRequestStatus? Status = null,
    RecruitmentRequestContext? RequestContext = null,
    Guid? DepartmentId = null,
    Guid? StoreId = null,
    Guid? RequestedByUserId = null
) : IRequest<QueryResult<RecruitmentRequestResponse>>;
