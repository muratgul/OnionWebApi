namespace OnionWebApi.Application.Helpers;
public static class PaginationHelper
{

    public static PaginatedResult<IEnumerable<T>> CreatePaginatedResponse<T>(bool isDynamic, IEnumerable<T> data, PaginationFilter paginationFilter, int totalRecords, IUriService uriService, string route, string fields = "")
    {
        var pageNumber = paginationFilter.PageNumber < 1 ? 1 : paginationFilter.PageNumber;
        var pageSize = paginationFilter.PageSize < 1 ? 10 : paginationFilter.PageSize;

        if (!isDynamic)
        {
            data = data.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }

        var totalPages = totalRecords == 0 ? 0 : (int)Math.Ceiling(totalRecords / (double)pageSize);

        if (pageNumber > totalPages && totalPages > 0)
        {
            pageNumber = totalPages;
        }


        var response = new PaginatedResult<IEnumerable<T>>(data, pageNumber, pageSize)
        {
            TotalPages = totalPages,
            TotalRecords = totalRecords,
            FirstPage = uriService.GeneratePageRequestUri(new PaginationFilter(1, pageSize), route, fields),
            LastPage = uriService.GeneratePageRequestUri(new PaginationFilter(totalPages, pageSize), route, fields),
            NextPage = pageNumber < totalPages
           ? uriService.GeneratePageRequestUri(new PaginationFilter(pageNumber + 1, pageSize), route, fields)
           : null,
            PreviousPage = pageNumber > 1
           ? uriService.GeneratePageRequestUri(new PaginationFilter(pageNumber - 1, pageSize), route, fields)
           : null
        };

        return response;
    }

    public static PaginatedResult<IEnumerable<ExpandoObject>> PaginatedResultWithFields<T>(IEnumerable<T> result, int totalRecords, IUriService uriService, int pageSize, int pageNumber, string route, string fields)
    {
        var shapedData = result.ShapeDataList(fields);

        PaginationFilter paginationFilter = new()
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
        };

        return PaginationHelper.CreatePaginatedResponse(true, shapedData, paginationFilter, totalRecords, uriService, route, fields);
    }
}
public class PaginationFilter
{
    private const int MaxPageSize = 100;
    private const int DefaultPageSize = 10;

    private int _pageNumber = 1;
    private int _pageSize = DefaultPageSize;

    public PaginationFilter()
    {
    }

    public PaginationFilter(int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    public int PageNumber
    {
        get => _pageNumber;
        set => _pageNumber = value < 1 ? 1 : value;
    }

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value switch
        {
            < 1 => DefaultPageSize,
            > MaxPageSize => MaxPageSize,
            _ => value
        };
    }

    public int Skip => (PageNumber - 1) * PageSize;
    public int Take => PageSize;
}