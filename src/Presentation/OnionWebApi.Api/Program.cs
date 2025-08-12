var builder = WebApplication.CreateBuilder(args);

builder.Services.EnvironmentRegister(builder.Configuration, builder);
builder.RegisterServices(typeof(Program));
var app = builder.Build();
app.RegisterPipelineComponents(typeof(Program));
app.Run();

