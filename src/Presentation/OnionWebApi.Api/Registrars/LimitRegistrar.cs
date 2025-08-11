namespace OnionWebApi.Api.Registrars;

public class LimitRegistrar : IWebApplicationBuilderRegistrar, IWebApplicationRegistrar
{
    public void RegisterServices(WebApplicationBuilder builder)
    {
        var rateLimitConfig = builder.Configuration.GetSection("RateLimiting").Get<RateLimitSettings>();

        if (rateLimitConfig is { Enabled: true })
        {
            builder.Services.AddRateLimiter(options =>
            {
                options.AddFixedWindowLimiter("Fixed", opt =>
                {
                    opt.PermitLimit = rateLimitConfig.PermitLimit;
                    opt.Window = TimeSpan.FromSeconds(rateLimitConfig.WindowSeconds);
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    opt.QueueLimit = rateLimitConfig.QueueLimit;
                });
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });
        }
    }

    public void RegisterPipelineComponents(WebApplication app)
    {
        var rateLimitConfig = app.Configuration.GetSection("RateLimiting").Get<RateLimitSettings>();
        if (rateLimitConfig is { Enabled: true })
        {
            app.UseRateLimiter();
        }
    }
}
