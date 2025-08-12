var builder = WebApplication.CreateBuilder(args);

var env = builder.Environment.EnvironmentName;
var envFile = File.Exists($".env.{env.ToLower()}") ? $".env.{env.ToLower()}" : ".env";

DotEnv.Load(new DotEnvOptions(envFilePaths: new[] { envFile }, overwriteExistingVars: true));


builder.Configuration.AddEnvironmentVariables();

builder.RegisterServices(typeof(Program));
var app = builder.Build();
app.RegisterPipelineComponents(typeof(Program));
app.Run();

