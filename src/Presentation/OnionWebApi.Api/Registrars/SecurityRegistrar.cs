namespace OnionWebApi.Api.Registrars;

public class SecurityRegistrar : IWebApplicationBuilderRegistrar, IWebApplicationRegistrar
{
    public void RegisterServices(WebApplicationBuilder builder)
    {
        //KeyCloak
        //builder.Services.AddKeycloakWebApiAuthentication(builder.Configuration);
        //builder.Services.AddAuthorization().AddKeycloakAuthorization(builder.Configuration);

        #region Cors
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAny", x =>
            {
                x.AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(isOriginAllowed: _ => true)
                .AllowCredentials()
                .WithExposedHeaders("WWW-Authenticate");
            });
        });
        #endregion

        #region RateLimit
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
        #endregion

        builder.Services.AddResponseCompression(opt =>
        {
            opt.EnableForHttps = true;
        });


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
