namespace OnionWebApi.Api.Registrars;

public static class EnvironmentRegistrar
{
    public static void EnvironmentRegister(this IServiceCollection services, IConfiguration configuration, WebApplicationBuilder builder)
    {
        var env = builder.Environment;

        builder.Configuration.SetBasePath(env.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
    }
}
