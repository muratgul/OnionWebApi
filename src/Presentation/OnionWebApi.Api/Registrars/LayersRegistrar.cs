namespace OnionWebApi.Api.Registrars;

public class LayersRegistrar : IWebApplicationBuilderRegistrar
{
    public void RegisterServices(WebApplicationBuilder builder)
    {
        builder.Services.AddPersistence(builder.Configuration);
        builder.Services.AddApplication();
        builder.Services.AddCustomMapper();
        builder.Services.AddInfrastructure(builder.Configuration);
    }
}
