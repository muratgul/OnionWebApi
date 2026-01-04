namespace OnionWebApi.Api.Registrars;

public class MessagingRegistrar : IWebApplicationBuilderRegistrar
{
    public void RegisterServices(WebApplicationBuilder builder)
    {
        var rabbitMQSettings = builder.Configuration
            .GetSection("RabbitMQ")
            .Get<RabbitMQSettings>();

        builder.Services.Configure<RabbitMQSettings>(
            builder.Configuration.GetSection("RabbitMQ"));

        builder.Services.AddMassTransit(opt =>
        {
            // Register Consumers
            opt.AddConsumer<BrandMessageConsumer>();

            // Health Check Configuration
            opt.ConfigureHealthCheckOptions(cfg =>
            {
                cfg.Name = "MassTransit";
                cfg.MinimalFailureStatus = HealthStatus.Unhealthy;
                cfg.Tags.Add("health");
            });

            if (rabbitMQSettings?.Enabled == true)
            {
                opt.UsingRabbitMq((context, cfg) =>
                {
                    // RabbitMQ Host Configuration
                    cfg.Host(
                        rabbitMQSettings.HostName ?? "localhost",
                        rabbitMQSettings.VirtualHost ?? "/",
                        h =>
                        {
                            h.Username(rabbitMQSettings.UserName ?? "guest");
                            h.Password(rabbitMQSettings.Password ?? "guest");
                        });

                    // Receive Endpoint Configuration
                    cfg.ReceiveEndpoint("brand-message-queue", e =>
                    {
                        e.ConfigureConsumer<BrandMessageConsumer>(context);

                        // Retry Policy
                        e.UseMessageRetry(r =>
                        {
                            r.Incremental(5, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10));
                        });

                        // Circuit Breaker
                        e.UseCircuitBreaker(cb =>
                        {
                            cb.TrackingPeriod = TimeSpan.FromMinutes(1);
                            cb.TripThreshold = 15;
                            cb.ActiveThreshold = 10;
                            cb.ResetInterval = TimeSpan.FromMinutes(5);
                        });

                        // Concurrency Limit (Optional)
                        e.PrefetchCount = 16;
                        e.ConcurrentMessageLimit = 8;
                    });
                });
            }
            else
            {
                opt.UsingInMemory((context, cfg) =>
                {
                    cfg.ConfigureEndpoints(context);
                });
            }
        });
    }
}
