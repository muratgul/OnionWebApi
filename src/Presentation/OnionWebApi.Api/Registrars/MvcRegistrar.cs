﻿using System.Text.Json.Serialization;

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
        builder.Services.AddScoped<ICacheService, HybridCacheService>();

        // HybridCache yapılandırması - Resilience özellikleri ile
        builder.Services.AddHybridCache(options =>
        {
            options.MaximumPayloadBytes = 1024 * 1024; // 1MB
            options.MaximumKeyLength = 1024;
            options.DefaultEntryOptions = new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(30),
                LocalCacheExpiration = TimeSpan.FromMinutes(5), // L1 cache daha kısa süreli
            };
        });

        // Redis distributed cache - Resilient configuration
        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = builder.Configuration.GetConnectionString("Redis");

            // Redis connection resilience settings
            options.ConfigurationOptions = new StackExchange.Redis.ConfigurationOptions
            {
                EndPoints = { builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379" },
                AbortOnConnectFail = false, // Redis bağlanamadığında app'i durdurma
                ConnectTimeout = 5000, // 5 saniye connection timeout
                SyncTimeout = 3000, // 3 saniye sync timeout
                ConnectRetry = 3, // 3 kere retry
                ReconnectRetryPolicy = new StackExchange.Redis.ExponentialRetry(1000), // Exponential backoff
            };
        });

        builder.Services.AddScoped<ICacheService, HybridCacheService>();

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

        builder.Services.AddHealthChecks().
            AddSqlServer(connectionString: builder.Configuration.GetConnectionString("DefaultConnection")!, name: "SqlServer")
            .AddRedis(builder.Configuration["RedisCacheSettings:ConnectionString"]!, name: "Redis")
            .AddUrlGroup(new Uri("https://www.tcmb.gov.tr/kurlar/kurlar_tr.html"), name: "External Api");
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
