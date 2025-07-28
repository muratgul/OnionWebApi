using OnionWebApi.Application.Helpers;

namespace OnionWebApi.Application.Utilities.URI;
public interface IUriService
{
    Uri GeneratePageRequestUri(PaginationFilter filter, string route, string fields);
}
