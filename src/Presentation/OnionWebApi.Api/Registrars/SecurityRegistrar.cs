using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Options;
using System.IO.Compression;

namespace OnionWebApi.Api.Registrars;

public class SecurityRegistrar : IWebApplicationBuilderRegistrar, IWebApplicationRegistrar
{

    public void RegisterServices(WebApplicationBuilder builder)
    {
        //KeyCloak
        //builder.Services.AddKeycloakWebApiAuthentication(builder.Configuration);
        //builder.Services.AddAuthorization().AddKeycloakAuthorization(builder.Configuration);

        #region Cors
        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAny", x =>
                {
                    x.AllowAnyMethod()
                     .AllowAnyHeader()
                     .SetIsOriginAllowed(_ => true)
                     .AllowCredentials()
                     .WithExposedHeaders("WWW-Authenticate");
                });
            });
        }
        else
        {
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAny", x =>
                {
                    x.WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? Array.Empty<string>())
                     .AllowAnyMethod()
                     .AllowAnyHeader()
                     .AllowCredentials()
                     .WithExposedHeaders("WWW-Authenticate");
                });
            });
        }
        #endregion

        #region RateLimit
        builder.Services.Configure<RateLimitSettings>(builder.Configuration.GetSection("RateLimiting"));

        var rateLimitConfig = builder.Configuration.GetSection("RateLimiting").Get<RateLimitSettings>();

        if (rateLimitConfig?.Enabled == true)
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

                options.OnRejected = async (context, token) =>
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

                    if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                    {
                        await context.HttpContext.Response.WriteAsJsonAsync(new
                        {
                            error = "Too many requests. Please try again later.",
                            retryAfter = $"{retryAfter.TotalSeconds} seconds"
                        }, cancellationToken: token);
                    }
                };
            });
        }
        #endregion

        #region Response Compression
        builder.Services.AddResponseCompression(opt =>
        {
            opt.EnableForHttps = true;
            opt.Providers.Add<BrotliCompressionProvider>();
            opt.Providers.Add<GzipCompressionProvider>();
            opt.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/json", "text/plain", "text/html" });
        });

        builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Fastest;
        });

        builder.Services.Configure<GzipCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Optimal;
        });
        #endregion
    }

    public void RegisterPipelineComponents(WebApplication app)
    {
        var rateLimitConfig = app.Services.GetService<IOptions<RateLimitSettings>>()?.Value;

        if (rateLimitConfig?.Enabled == true)
        {
            app.UseRateLimiter();
        }
    }

}
