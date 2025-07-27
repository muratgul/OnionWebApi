using UnionWebApi.Api.Registrars.Interfaces;
using UnionWebApi.Application.Utilities.URI;

namespace UnionWebApi.Api.Registrars;

public class DependencyInjectionRegistrar : IWebApplicationBuilderRegistrar
{
    public void RegisterServices(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IUriService>(provider =>
        {
            return new UriManager("http://localhost:5193/api/");
        });
    }
}
