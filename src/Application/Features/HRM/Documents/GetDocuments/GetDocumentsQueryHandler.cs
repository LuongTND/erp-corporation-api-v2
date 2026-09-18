namespace Application;

public sealed class GetDocumentsQueryHandler(IUnitOfWork unitOfWork, IBlobStorageService blobStorage)
    : IRequestHandler<GetDocumentsQuery, IEnumerable<EmployeeDocumentResponse>>
{
    private const string Container = "employee-documents";
    private static readonly TimeSpan WarnWindow = TimeSpan.FromDays(30);

    public async Task<IEnumerable<EmployeeDocumentResponse>> Handle(GetDocumentsQuery query, CancellationToken ct)
    {
        var docs = await unitOfWork.Repository<EmployeeDocument>()
            .GetAllAsync(d => d.UserId == query.UserId, ct);

        if (query.SelfView)
            docs = docs.Where(d => d.CreatedBy == query.CallerId || d.IsVisibleToEmployee).ToList();

        return docs.OrderByDescending(d => d.CreatedAt).Select(Map);
    }

    private EmployeeDocumentResponse Map(EmployeeDocument d)
    {
        var now = DateTimeOffset.UtcNow;
        var expired = d.ExpiryDate.HasValue && d.ExpiryDate.Value < now;
        var expiringSoon = !expired && d.ExpiryDate.HasValue && d.ExpiryDate.Value < now.Add(WarnWindow);

        return new EmployeeDocumentResponse
        {
            Id = d.Id,
            Category = d.Category.ToString(),
            CustomName = d.CustomName,
            DisplayName = d.Category == DocumentCategory.Other ? (d.CustomName ?? "Khác") : CategoryLabel(d.Category),
            OriginalFileName = d.OriginalFileName,
            ContentType = d.ContentType,
            FileSizeBytes = d.FileSizeBytes,
            FileUrl = blobStorage.GetUrl(Container, d.BlobName),
            IssuedDate = d.IssuedDate,
            ExpiryDate = d.ExpiryDate,
            Notes = d.Notes,
            CreatedAt = d.CreatedAt,
            IsExpired = expired,
            IsExpiringSoon = expiringSoon,
            IsVisibleToEmployee = d.IsVisibleToEmployee,
            UploadedById = d.CreatedBy,
        };
    }

    private static string CategoryLabel(DocumentCategory cat) => cat switch
    {
        DocumentCategory.IdentityCard          => "CCCD / CMND",
        DocumentCategory.HouseholdBook         => "Hộ khẩu / KT3",
        DocumentCategory.JudicialRecord        => "Lý lịch tư pháp",
        DocumentCategory.HealthCertificate     => "Phiếu khám sức khỏe",
        DocumentCategory.RecruitmentDecision   => "Quyết định tuyển dụng",
        DocumentCategory.ProbationContract     => "Hợp đồng thử việc",
        DocumentCategory.LaborContract         => "Hợp đồng lao động",
        DocumentCategory.Degree                => "Bằng tốt nghiệp",
        DocumentCategory.Certificate           => "Chứng chỉ",
        DocumentCategory.DriversLicense        => "Giấy phép lái xe",
        DocumentCategory.FoodSafetyCertificate => "Chứng chỉ ATTP",
        DocumentCategory.AppointmentDecision   => "Quyết định bổ nhiệm",
        DocumentCategory.TransferDecision      => "Quyết định điều chuyển",
        _                                      => "Khác",
    };
}
