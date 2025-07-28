using OnionWebApi.Api.Registrars.Interfaces;
using OnionWebApi.Persistence.Context;

namespace OnionWebApi.Api.Registrars;

public class ThirdPartyRegistrar : IWebApplicationBuilderRegistrar
{
    public void RegisterServices(WebApplicationBuilder builder)
    {
        builder.Services.AddCap(options =>
        {
            options.UseEntityFramework<AppDbContext>();
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")!);
            options.UseRabbitMQ(rabbitMQ =>
            {
                rabbitMQ.HostName = "localhost";
                rabbitMQ.Port = 5672;
                rabbitMQ.UserName = "guest";
                rabbitMQ.Password = "guest";
            });
            options.UseDashboard();
        });
    }   
}