namespace Application;

public sealed record GetDocumentsQuery(Guid UserId) : IRequest<IEnumerable<EmployeeDocumentResponse>>;
