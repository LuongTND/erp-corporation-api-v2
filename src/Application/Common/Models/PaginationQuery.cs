namespace Application.Common.Models;

public class PaginationQuery
{
    public const int DefaultPageSize = 10;
    public const int MaxPageSize = 100;

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = DefaultPageSize;

    public (int Page, int PageSize) Normalize()
    {
        var page = Page < 1 ? 1 : Page;
        var pageSize = PageSize < 1 ? DefaultPageSize : Math.Min(PageSize, MaxPageSize);
        return (page, pageSize);
    }
}
