using System.Dynamic;
using UnionWebApi.Application.Utilities.Results;
using UnionWebApi.Application.Utilities.URI;

namespace UnionWebApi.Application.Helpers;
public static class PaginationHelper
{

    public static PaginatedResult<IEnumerable<T>> CreatePaginatedResponse<T>(bool isDynamic, IEnumerable<T> data, PaginationFilter paginationFilter, int totalRecords, IUriService uriService, string route, string fields = "")
    {
        var pageNumber = paginationFilter.PageNumber < 1 ? 1 : paginationFilter.PageNumber;
        var pageSize = paginationFilter.PageSize < 1 ? 10 : paginationFilter.PageSize;

        if (!isDynamic)
            data = data.Skip((pageNumber - 1) * pageSize).Take(pageSize);

        var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

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
    public PaginationFilter()
    {
        PageNumber = 1;
        PageSize = 10;
    }

    public PaginationFilter(int pageNumber, int pageSize)
    {
        PageNumber = pageNumber < 1 ? 1 : pageNumber;
        PageSize = pageSize > 10 ? 10 : pageSize;
    }

    public int PageNumber
    {
        get; set;
    }
    public int PageSize
    {
        get; set;
    }
}