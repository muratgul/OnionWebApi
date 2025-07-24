using Microsoft.Extensions.DependencyInjection;
using UnionWebApi.Application.Interfaces.AutoMapper;

namespace UnionWebApi.Mapper;

public static class Registration
{
    public static void AddCustomMapper(this IServiceCollection services)
    {
        services.AddSingleton<IMapper, AutoMapper.Mapper>();
    }
}
