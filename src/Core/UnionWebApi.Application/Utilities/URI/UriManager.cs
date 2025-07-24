using Microsoft.AspNetCore.WebUtilities;
using UnionWebApi.Application.Helpers;

namespace UnionWebApi.Application.Utilities.URI;
public class UriManager : IUriService
{
    private readonly string _baseUri;
    public UriManager(string baseUri)
    {
        _baseUri = baseUri;
    }
    public Uri GeneratePageRequestUri(PaginationFilter filter, string route, string fields)
    {
        var endpointUri = new Uri(string.Concat(_baseUri, route));
        var modifiedUri = QueryHelpers.AddQueryString(endpointUri.ToString(), "pageNumber", filter.PageNumber.ToString());
        modifiedUri = QueryHelpers.AddQueryString(modifiedUri, "pageSize", filter.PageSize.ToString());

        if (!string.IsNullOrWhiteSpace(fields))
            modifiedUri = QueryHelpers.AddQueryString(modifiedUri, "Fields", fields);

        return new Uri(modifiedUri);
    }
}
