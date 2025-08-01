namespace OnionWebApi.Api.Registrars;

public class DependencyInjectionRegistrar : IWebApplicationBuilderRegistrar
{
    public void RegisterServices(WebApplicationBuilder builder)
    {
        var uri = builder.Configuration["BaseUri"];        
        builder.Services.AddScoped<IUriService>(provider =>
        {
            return new UriManager(uri!);
        });
    }
}
