namespace Domain;

public class ContractTemplate : AuditableEntityBase<Guid>
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string BlobName { get; set; } = null!;         // path trong Azure Blob container
    public string OriginalFileName { get; set; } = null!; // tên file gốc khi upload
    public bool IsActive { get; set; } = true;

    public ICollection<EmploymentContract> Contracts { get; set; } = [];
}
