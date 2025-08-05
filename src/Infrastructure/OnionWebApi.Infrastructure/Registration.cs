using OnionWebApi.Application.Interfaces.Messaging;
using OnionWebApi.Infrastructure.Email;
using OnionWebApi.Infrastructure.Messaging;

namespace OnionWebApi.Infrastructure;
public static class Registration
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<TokenSettings>(configuration.GetSection("JWT"));
        services.AddTransient<ITokenService, TokenService>();
        services.AddScoped<IMassTransitSend, MassTransitSend>();
        
        services.Configure<RedisCacheSettings>(configuration.GetSection("RedisCacheSettings"));
        services.AddSingleton<IRedisCacheSettings>(sp =>
        {
            return sp.GetRequiredService<IOptions<RedisCacheSettings>>().Value;
        });
        services.AddTransient<IRedisCacheService, RedisCacheService>();

        services.AddHostedService<BackgroundEmailService>();

        services.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opt =>
        {
            opt.Authority = configuration["JWT:Authority"];
            opt.Audience = configuration["JWT:Audience"];
            opt.RequireHttpsMetadata = Convert.ToBoolean(configuration["JWT:RequireHttpsMetadata"]);
            opt.SaveToken = true;
            opt.TokenValidationParameters = new TokenValidationParameters()
            {

                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"])),
                ValidateLifetime = false,
                ValidIssuer = configuration["JWT:ValidIssuer"],
                ValidAudience = configuration["JWT:Audience"],
                ClockSkew = TimeSpan.Zero
            };
        });

        // Ensure the required package is installed: Microsoft.Extensions.Caching.StackExchangeRedis
        services.AddStackExchangeRedisCache(opt =>
        {
            opt.Configuration = configuration["RedisCacheSettings:ConnectionString"];
            opt.InstanceName = configuration["RedisCacheSettings:InstanceName"];
        });
    }
}
