using Application.Common.Models;
using AutoMapper;

namespace Application.Common.Mapping;

public static class PaginationMapper
{
    public static PaginatedResult<TDto> Map<TSource, TDto>(
        PaginatedResult<TSource> source,
        IMapper mapper)
    {
        return new PaginatedResult<TDto>
        {
            Items = mapper.Map<List<TDto>>(source.Items),
            Page = source.Page,
            PageSize = source.PageSize,
            TotalCount = source.TotalCount,
        };
    }
}
