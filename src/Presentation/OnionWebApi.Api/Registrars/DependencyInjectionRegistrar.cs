namespace OnionWebApi.Api.Registrars;

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
