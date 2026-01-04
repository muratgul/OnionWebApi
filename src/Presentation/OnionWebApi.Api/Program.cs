var builder = WebApplication.CreateBuilder(args);

builder.Services.EnvironmentRegister(builder.Configuration, builder);
builder.RegisterServices(typeof(Program));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    //await IdentityDataSeeder.SeedAdminUserAsync(scope.ServiceProvider);
}

app.RegisterPipelineComponents(typeof(Program));
app.Run();

