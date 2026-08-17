namespace Contract;

public sealed class KpiSummaryResponse
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public int Month { get; set; }
    public int Year { get; set; }
    public decimal TotalScore { get; set; }
    public IReadOnlyList<KpiEntryResponse> Entries { get; set; } = [];
}
