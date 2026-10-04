using Renmember.Api.Health;
using Renmember.Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Renmember")
    ?? throw new InvalidOperationException("Connection string 'Renmember' não configurada.");

builder.Services.AddInfrastructure(connectionString);
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks().AddCheck<DatabaseHealthCheck>("database");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await app.Services.AplicarMigrationsAsync();
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapHealthChecks("/health");

await app.RunAsync();
