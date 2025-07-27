using Serilog;
using UnionWebApi.Api.Registrars.Interfaces;

namespace UnionWebApi.Api.Registrars
{
    public class SeriLogRegistar : IWebApplicationBuilderRegistrar
    {
        public void RegisterServices(WebApplicationBuilder builder)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .CreateLogger();

            builder.Host.UseSerilog();
        }
    }
}
