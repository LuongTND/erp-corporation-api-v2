namespace Domain;

public class PayrollRun : AuditableEntityBase<Guid>, ISoftDeletable
{
    public int Month { get; set; }
    public int Year { get; set; }
    public PayrollRunStatus Status { get; set; } = PayrollRunStatus.Draft;
    public string? Note { get; set; }

    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }

    public ICollection<PayrollEntry> Entries { get; set; } = [];
}
