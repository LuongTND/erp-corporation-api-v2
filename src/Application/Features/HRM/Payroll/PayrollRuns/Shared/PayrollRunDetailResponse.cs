namespace Application;

public sealed class PayrollRunDetailResponse
{
    public Guid Id { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Note { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public IReadOnlyList<PayrollEntryResponse> Entries { get; set; } = [];
}
