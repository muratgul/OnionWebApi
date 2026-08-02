using OnionWebApi.Api.Controllers.v1;
using ZiggyCreatures.Caching.Fusion;
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
        var cacheSettings = builder.Configuration.GetSection("CacheSettings").Get<CacheSettings>();

        if(cacheSettings?.Enabled == true)
        {
            builder.Services.AddSingleton<ICacheService, FusionCacheService>();
            
            var fusionBuilder = builder.Services.AddFusionCache()
                .WithDefaultEntryOptions(new FusionCacheEntryOptions
                {
                    Duration = TimeSpan.FromMinutes(30),
                    IsFailSafeEnabled = true,
                    FailSafeMaxDuration = TimeSpan.FromHours(2),
                    FailSafeThrottleDuration = TimeSpan.FromSeconds(30),
                    FactorySoftTimeout = TimeSpan.FromMilliseconds(100),
                    EagerRefreshThreshold = 0.8f
                })
                .WithSerializer(new ZiggyCreatures.Caching.Fusion.Serialization.SystemTextJson.FusionCacheSystemTextJsonSerializer());

            if (redisCacheSettings?.Enabled == true)
            {
                fusionBuilder
                    .WithDistributedCache(new Microsoft.Extensions.Caching.StackExchangeRedis.RedisCache(new Microsoft.Extensions.Caching.StackExchangeRedis.RedisCacheOptions
                    {
                        Configuration = redisCacheSettings.ConnectionString,
                        InstanceName = redisCacheSettings.InstanceName,
                        ConfigurationOptions = new StackExchange.Redis.ConfigurationOptions
                        {
                            EndPoints = { redisCacheSettings.ConnectionString },
                            AbortOnConnectFail = false,
                            ConnectTimeout = 1000,
                            SyncTimeout = 1000,
                            ConnectRetry = 3,
                            ReconnectRetryPolicy = new StackExchange.Redis.ExponentialRetry(1000),
                        }
                    }))
                    .WithBackplane(new ZiggyCreatures.Caching.Fusion.Backplane.StackExchangeRedis.RedisBackplane(new ZiggyCreatures.Caching.Fusion.Backplane.StackExchangeRedis.RedisBackplaneOptions
                    {
                        Configuration = redisCacheSettings.ConnectionString
                    }));
            }
        }
        else 
        {
            builder.Services.AddScoped<ICacheService, NullCacheService>();
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