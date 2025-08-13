namespace OnionWebApi.Api.Registrars;

public class MvcWebAppRegistrar : IWebApplicationRegistrar
{
    public void RegisterPipelineComponents(WebApplication app)
    {
        app.MapHealthChecks("/healthapi", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
            ResultStatusCodes =
                {
                    [HealthStatus.Healthy] = StatusCodes.Status200OK,
                    [HealthStatus.Degraded] = StatusCodes.Status500InternalServerError,
                    [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable,
                },
        });
        app.UseHealthChecksUI(options => { options.UIPath = "/health"; });
        app.UseMiddleware<ExceptionMiddleware>();
        app.UseMiddleware<IdempotencyMiddleware>();
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference(option =>
            {
                option
                .WithTitle("Union Web API")
                .WithTheme(ScalarTheme.Default)
                .WithDarkMode(false);
            });
        }
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers().RequireRateLimiting("Fixed");
        app.UseCors("AllowAny");
        app.MapHub<GlobalHub>("/hubs/global");
    }
}
