namespace Application;

/// <param name="UserId">Tài liệu của nhân viên nào</param>
/// <param name="SelfView">true = nhân viên tự xem — áp dụng visibility filter</param>
/// <param name="CallerId">Id người gọi, dùng khi SelfView = true</param>
public sealed record GetDocumentsQuery(
    Guid UserId,
    bool SelfView = false,
    Guid? CallerId = null
) : IRequest<IEnumerable<EmployeeDocumentResponse>>;
