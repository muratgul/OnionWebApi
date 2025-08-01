namespace OnionWebApi.Api.Registrars;

public class ThirdPartyRegistrar : IWebApplicationBuilderRegistrar
{
    public void RegisterServices(WebApplicationBuilder builder)
    {        
        builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQ"));

            builder.Services.AddMassTransit(opt =>
            {
                // Register MassTransit Consumer
                opt.AddConsumer<BrandMessageConsumer>();

                opt.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(builder.Configuration["RabbitMQ:HostName"], "/", h =>
                    {
                        h.Username(builder.Configuration["RabbitMQ:UserName"]!);
                        h.Password(builder.Configuration["RabbitMQ:Password"]!);
                    });

                    cfg.ReceiveEndpoint("brand-message-queue", e =>
                    {
                        e.Consumer<BrandMessageConsumer>(context);
                    });

                    cfg.UseMessageRetry(r =>
                    {
                        r.Incremental(5, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10));
                    });

                    cfg.ConfigureEndpoints(context);
                });

            });       
    }   
}