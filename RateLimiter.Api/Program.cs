using RateLimiter.Api.Middleware;
using RateLimiter.Api.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<RateLimitingOptions>(builder.Configuration.GetSection("RateLimiting"));

var app = builder.Build();

app.UseMiddleware<RateLimitingMiddleware>();

app.MapGet("/", () => "Hello World!");

app.Run();