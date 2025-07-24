using UnionWebApi.Application.Helpers;

namespace UnionWebApi.Application.Utilities.URI;
public interface IUriService
{
    Uri GeneratePageRequestUri(PaginationFilter filter, string route, string fields);
}
