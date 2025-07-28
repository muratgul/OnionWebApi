using Microsoft.Extensions.DependencyInjection;
using OnionWebApi.Application.Interfaces.AutoMapper;

namespace OnionWebApi.Mapper;

public static class Registration
{
    public static void AddCustomMapper(this IServiceCollection services)
    {
        services.AddSingleton<IMapper, AutoMapper.Mapper>();
    }
}
