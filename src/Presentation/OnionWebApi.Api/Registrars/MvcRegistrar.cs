using OnionWebApi.Api.Controllers.v1;
using OnionWebApi.Infrastructure.Cache;
using System.Text.Json.Serialization;

namespace OnionWebApi.Api.Registrars;

public class MvcRegistrar : IWebApplicationBuilderRegistrar
{
    public class UpperCaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return name;

            return char.ToUpper(name[0]) + name.Substring(1);
        }
    }
    public void RegisterServices(WebApplicationBuilder builder)
    {
        var redisCacheSettings = builder.Configuration.GetSection("RedisCacheSettings").Get<RedisCacheSettings>();

        if (redisCacheSettings is not null && redisCacheSettings.Enabled)
        {
            builder.Services.AddScoped<ICacheService, HybridCacheService>();

            builder.Services.AddHybridCache(options =>
            {
                options.MaximumPayloadBytes = 1024 * 1024;
                options.MaximumKeyLength = 1024;
                options.DefaultEntryOptions = new HybridCacheEntryOptions
                {
                    Expiration = TimeSpan.FromMinutes(30),
                    LocalCacheExpiration = TimeSpan.FromMinutes(5),
                };
            });

            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisCacheSettings.ConnectionString;
                options.ConfigurationOptions = new StackExchange.Redis.ConfigurationOptions
                {
                    EndPoints = { redisCacheSettings.ConnectionString },
                    AbortOnConnectFail = false,
                    ConnectTimeout = 1000,
                    SyncTimeout = 1000,
                    ConnectRetry = 3,
                    ReconnectRetryPolicy = new StackExchange.Redis.ExponentialRetry(1000),
                };
            });
        }
        else
        {
            builder.Services.AddScoped<ICacheService, InMemoryCacheService>();
        }


        builder.Services.AddApiVersioning(options =>
        {
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.ReportApiVersions = true;
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
        });

        builder.Services.AddMemoryCache();
        builder.Services.AddSignalR();
        builder.Services.AddScoped<INotificationService, NotificationService>();

        var healthChecksBuilder = builder.Services.AddHealthChecks().
            AddSqlServer(connectionString: builder.Configuration.GetConnectionString("DefaultConnection")!, name: "SqlServer");

        if (redisCacheSettings is not null && redisCacheSettings.Enabled)
        {
            healthChecksBuilder.AddRedis(redisCacheSettings.ConnectionString, name: "Redis");
        }

        healthChecksBuilder.AddUrlGroup(new Uri("https://www.tcmb.gov.tr/kurlar/kurlar_tr.html"), name: "External Api");

        builder.Services.AddHealthChecksUI(setup =>
        {
            setup.AddHealthCheckEndpoint("HealthCheck API", "/healthapi");
            setup.SetEvaluationTimeInSeconds(60);
            setup.SetMinimumSecondsBetweenFailureNotifications(120);
        }).AddInMemoryStorage();
        builder.Services.AddControllersWithViews();
        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                options.JsonSerializerOptions.WriteIndented = true;

            }).AddOData(opt =>
            {
                opt
                .Select()
                .Filter()
                .Count()
                .Expand()
                .OrderBy()
                .SetMaxTop(null)
                .AddRouteComponents("odata", AppODataController.GetEdmModel());
            });
    }
}