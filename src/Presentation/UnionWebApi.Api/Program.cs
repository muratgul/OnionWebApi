using Serilog;
using UnionWebApi.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.RegisterServices(typeof(Program));
var app = builder.Build();
app.RegisterPipelineComponents(typeof(Program));
app.Run();
