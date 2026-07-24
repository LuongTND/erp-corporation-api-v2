namespace Contract;

public class QueryInfo
{
    public int Top { get; set; } = AppConstants.DefaultPageTop;
    public int Skip { get; set; } = AppConstants.DefaultPageSkip;
    public string? SearchText { get; set; }
    public bool IsActive { get; set; } = true;
    public string? OrderBy { get; set; } = AppConstants.DefaultOrderBy;
    public OrderType OrderType { get; set; } = OrderType.Descending;
    public bool NeedTotalCount { get; set; } = AppConstants.DefaultNeedTotalCount;
}
