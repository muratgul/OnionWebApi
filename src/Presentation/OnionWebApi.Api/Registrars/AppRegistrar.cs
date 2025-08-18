namespace OnionWebApi.Api.Registrars;

public class AppRegistrar : IWebApplicationBuilderRegistrar
{
    public void RegisterServices(WebApplicationBuilder builder)
    {
        builder.Services.Configure<EmailConfiguration>(builder.Configuration.GetSection("EmailConfiguration"));

        var uri = builder.Configuration["BaseUri"];        
        builder.Services.AddScoped<IUriService>(provider =>
        {
            return new UriManager(uri!);
        });
    }
}
