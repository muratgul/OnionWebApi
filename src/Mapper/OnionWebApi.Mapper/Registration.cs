using System.Reflection;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;

namespace OnionWebApi.Mapper;

public static class Registration
{
    public static void AddCustomMapper(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(Assembly.GetExecutingAssembly());

        //services.AddSingleton<IMapper, AutoMapper.Mapper>();
        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();
    }
}
