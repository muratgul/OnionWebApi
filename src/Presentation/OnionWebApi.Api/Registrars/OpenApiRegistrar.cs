namespace OnionWebApi.Api.Registrars;

public class OpenApiRegistrar : IWebApplicationBuilderRegistrar
{
    public void RegisterServices(WebApplicationBuilder builder)
    {
        builder.Services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                // API Bilgileri
                document.Info = new OpenApiInfo
                {
                    Title = "Union Web API",
                    Version = "v1",
                    Description = "ASP.NET Core Web API with Onion Architecture",
                    Contact = new OpenApiContact
                    {
                        Name = "Support Team",
                        Email = "support@yourcompany.com",
                        Url = new Uri("https://yourcompany.com")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT License",
                        Url = new Uri("https://opensource.org/licenses/MIT")
                    }
                };

                // Sunucu bilgileri (opsiyonel)
                document.Servers =
                    [
                        new() { Url = "https://api.yourcompany.com", Description = "Production" },
                        new() { Url = "https://staging-api.yourcompany.com", Description = "Staging" }
                    ];

                document.Components ??= new();
                document.Components.SecuritySchemes ??= new Dictionary<string, OpenApiSecurityScheme>();

                document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "JWT Authorization header using the Bearer scheme."
                };

                document.SecurityRequirements.Add(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });

                return Task.CompletedTask;
            });
        });
    }
}
