using OnionWebApi.Api.Registrars.Interfaces;
using OnionWebApi.Application;
using OnionWebApi.Persistence;
using OnionWebApi.Infrastructure;
using OnionWebApi.Mapper;


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
