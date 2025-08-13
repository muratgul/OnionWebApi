namespace OnionWebApi.Api.Registrars;

public static class EnvironmentRegistrar
{
    public static void EnvironmentRegister(this IServiceCollection services, IConfiguration configuration, WebApplicationBuilder builder)
    {
        var env = builder.Environment;

        builder.Configuration.SetBasePath(env.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

        //var env = builder.Environment.EnvironmentName;
        //var envFile = File.Exists($".env.{env.ToLower()}") ? $".env.{env.ToLower()}" : ".env";
        //DotEnv.Load(new DotEnvOptions(envFilePaths: new[] { envFile }, overwriteExistingVars: true));

        //builder.Configuration.AddEnvironmentVariables();
    }
}
