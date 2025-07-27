using UnionWebApi.Api.Registrars.Interfaces;
using UnionWebApi.Application;
using UnionWebApi.Persistence;
using UnionWebApi.Infrastructure;
using UnionWebApi.Mapper;


namespace UnionWebApi.Api.Registrars;

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
