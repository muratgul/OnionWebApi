

namespace OnionWebApi.Api.Registrars;

public class ThirdPartyRegistrar : IWebApplicationBuilderRegistrar
{
    public void RegisterServices(WebApplicationBuilder builder)
    {
        builder.Services.AddMassTransit(opt =>
        {
            opt.UsingRabbitMq((context, cfg)=>
            {
                cfg.Host(builder.Configuration["RabbitMQ:HostName"], "/", h =>
                {
                    h.Username(builder.Configuration["RabbitMQ:UserName"]!);
                    h.Password(builder.Configuration["RabbitMQ:Password"]!);                    
                });
                cfg.ConfigureEndpoints(context);
            });
        });
    }   
}