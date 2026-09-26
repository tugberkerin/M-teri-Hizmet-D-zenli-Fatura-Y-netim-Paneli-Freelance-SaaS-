using ClientPulse.Core;
using ClientPulse.Infrastructure;
using ClientPulse.Infrastructure.Data;
using ClientPulse.WebAPI.Endpoints;
using ClientPulse.WebAPI.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

// Global Exception Handler
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddProblemDetails();

// Configure JSON Options (String Enum Converter)
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});

// CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Core and Infrastructure DI
builder.Services.AddCore();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Seed Database
await DbSeeder.SeedAsync(app.Services);

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseCors("AllowAll");

app.UseExceptionHandler();

// Root route redirect to Scalar API Reference
app.MapGet("/", () => Results.Redirect("/scalar/v1"));

// Map Endpoints
app.MapClientEndpoints();
app.MapServiceEndpoints();
app.MapInvoiceEndpoints();
app.MapUptimeEndpoints();

app.Run();
