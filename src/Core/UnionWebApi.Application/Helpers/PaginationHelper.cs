using System.Dynamic;
using UnionWebApi.Application.Utilities.Results;
using UnionWebApi.Application.Utilities.URI;

namespace UnionWebApi.Application.Helpers;
public static class PaginationHelper
{

    public static PaginatedResult<IEnumerable<T>> CreatePaginatedResponse<T>(bool isDynamic, IEnumerable<T> data, PaginationFilter paginationFilter, int totalRecords, IUriService uriService, string route, string fields = "")
    {
        if (!isDynamic)
            data = data.Skip((paginationFilter.PageNumber - 1) * paginationFilter.PageSize).Take(paginationFilter.PageSize);

        int roundedTotalPages;
        var response = new PaginatedResult<IEnumerable<T>>(data, paginationFilter.PageNumber, paginationFilter.PageSize);
        var totalPages = totalRecords / (double)paginationFilter.PageSize;
        if (paginationFilter.PageNumber <= 0 || paginationFilter.PageSize <= 0)
        {
            roundedTotalPages = 1;
            paginationFilter.PageNumber = 1;
            paginationFilter.PageSize = 1;
        }
        else
        {
            roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));
        }

        response.NextPage = paginationFilter.PageNumber >= 1 && paginationFilter.PageNumber < roundedTotalPages
            ? uriService.GeneratePageRequestUri(new PaginationFilter(paginationFilter.PageNumber + 1, paginationFilter.PageSize), route, fields) : null;
        response.PreviousPage = paginationFilter.PageNumber - 1 >= 1 && paginationFilter.PageNumber <= roundedTotalPages ? uriService.GeneratePageRequestUri(
                    new PaginationFilter(paginationFilter.PageNumber - 1, paginationFilter.PageSize), route, fields) : null;
        response.FirstPage = uriService.GeneratePageRequestUri(new PaginationFilter(1, paginationFilter.PageSize), route, fields);
        response.LastPage = uriService.GeneratePageRequestUri(new PaginationFilter(roundedTotalPages, paginationFilter.PageSize), route, fields);
        response.TotalPages = roundedTotalPages;
        response.TotalRecords = totalRecords;
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