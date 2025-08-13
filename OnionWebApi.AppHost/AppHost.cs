using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);
var mssql = builder.AddSqlServer("mssql").WithDataVolume("mssql_datax").AddDatabase("APIDB");
var redis = builder.AddRedis("redis").WithDataVolume("redis_data");
var rabbitMq = builder.AddRabbitMQ("rabbitmq").WithDataVolume("rabbitmq_data");

builder.AddProject<Projects.OnionWebApi_Api>("onionwebapi-api")
    .WithReference(mssql)
    .WithReference(redis)
    .WithReference(rabbitMq)
    .WithEnvironment("RedisCacheSettings__ConnectionString", "redis:6379,abortConnect=false")
    .WithEnvironment("RabbitMQ__HostName", "rabbitmq")
    .WithEnvironment("RabbitMQ__UserName", "guest")
    .WithEnvironment("RabbitMQ__Password", "guest")
    .WaitFor(mssql)
    .WaitFor(redis)
    .WaitFor(rabbitMq);

builder.Build().Run();
