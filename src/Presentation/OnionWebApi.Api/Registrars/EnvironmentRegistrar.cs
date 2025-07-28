using OnionWebApi.Api.Registrars.Interfaces;

namespace OnionWebApi.Api.Registrars;

public class EnvironmentRegistrar : IWebApplicationBuilderRegistrar
{
    public void RegisterServices(WebApplicationBuilder builder)
    {
        var env = builder.Environment;

        builder.Configuration.SetBasePath(env.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
    }
}
