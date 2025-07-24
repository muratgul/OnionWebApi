namespace UnionWebApi.Api.Registrars.Interfaces;

public interface IWebApplicationBuilderRegistrar : IRegistrar
{
    public void RegisterServices(WebApplicationBuilder builder);
}
