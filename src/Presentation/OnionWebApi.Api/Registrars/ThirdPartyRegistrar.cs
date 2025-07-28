namespace OnionWebApi.Api.Registrars;

public class ThirdPartyRegistrar : IWebApplicationBuilderRegistrar
{
    public void RegisterServices(WebApplicationBuilder builder)
    {
        builder.Services.AddCap(options =>
        {
            options.UseEntityFramework<AppDbContext>();
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")!);
            options.UseRabbitMQ(rabbitMQ =>
            {
                rabbitMQ.HostName = builder.Configuration["RabbitMQ:HostName"]!;
                rabbitMQ.Port = int.Parse(builder.Configuration["RabbitMQ:Port"]!);
                rabbitMQ.UserName = builder.Configuration["RabbitMQ:UserName"]!;
                rabbitMQ.Password = builder.Configuration["RabbitMQ:Password"]!;
                rabbitMQ.VirtualHost = builder.Configuration["RabbitMQ:VirtualHost"]!;
            });
            options.UseDashboard();
        });
    }   
}