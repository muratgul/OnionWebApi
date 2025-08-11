namespace OnionWebApi.Api.Registrars;

public class LayersRegistrar : IWebApplicationBuilderRegistrar
{
    public void RegisterServices(WebApplicationBuilder builder)
    {
        builder.Services.AddPersistence(builder.Configuration, builder);
        builder.Services.AddApplication();
        builder.Services.AddInfrastructure(builder.Configuration);
    }
}
