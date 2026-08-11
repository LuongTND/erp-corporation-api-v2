namespace Domain;

public class SalaryRecord : AuditableEntityBase<Guid>
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public decimal HourlyRate { get; set; }
    public DateOnly EffectiveFrom { get; set; }
    public DateOnly? EffectiveTo { get; set; }  // null = đang hiệu lực

    public string? Reason { get; set; }         // "Học việc → Chính thức"
}
