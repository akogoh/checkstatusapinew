using System;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Options;
using checkstastusapinew.Services;
using checkstastusapinew;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHealthChecks();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register RiskDbService for accessing the RiskDatabase
builder.Services.AddScoped<RiskDbService>();


var app = builder.Build();

// Support running behind a reverse proxy (useful in production/container environments)
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// Configure the HTTP request pipeline.
// Temporarily always enable Swagger in all environments (Production included).
// Keep API-key protection if SWAGGER_API_KEY is set.
var enableSwaggerEnv = Environment.GetEnvironmentVariable("ENABLE_SWAGGER");

var swaggerApiKey = Environment.GetEnvironmentVariable("SWAGGER_API_KEY");

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "checkstatus API v1");
    c.RoutePrefix = "swagger";
});



if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();

// Basic security headers
app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["Referrer-Policy"] = "no-referrer";
    await next();
});

app.UseAuthorization();

app.MapControllers();

// Liveness and readiness probes
app.MapHealthChecks("/health");
app.MapHealthChecks("/ready");

app.Run();
