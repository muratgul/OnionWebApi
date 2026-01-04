namespace OnionWebApi.Api.Registrars;

public class MvcWebAppRegistrar : IWebApplicationRegistrar
{
    public void RegisterPipelineComponents(WebApplication app)
    {
        app.UseMiddleware<ExceptionMiddleware>();
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
        app.UseResponseCompression();
        app.UseHttpsRedirection();
        app.UseCors("AllowAny");
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseMiddleware<IdempotencyMiddleware>();
        app.MapControllers().RequireRateLimiting("Fixed");
        app.MapHub<GlobalHub>("/hubs/global");
    }
}
