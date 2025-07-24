using Scalar.AspNetCore;
using UnionWebApi.Api.Registrars.Interfaces;

namespace UnionWebApi.Api.Registrars;

public class MvcWebAppRegistrar : IWebApplicationRegistrar
{
    public void RegisterPipelineComponents(WebApplication app)
    {
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
        app.MapControllers();
        app.UseCors("AllowAny");
    }
}
