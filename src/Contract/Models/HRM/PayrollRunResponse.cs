namespace Contract;

public sealed class PayrollRunResponse
{
    public Guid Id { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Note { get; set; }
    public int EntryCount { get; set; }
    public decimal TotalNetPay { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
