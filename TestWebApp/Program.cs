using System.Reflection;
using RateLimiter;
using RateLimiter.Abstractions;
using RateLimiter.Fakes;
using RateLimiter.Models.RateLimits;
using TestWebApp.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<IUserActivityRepository, FakeUserActivityRepository>();
builder.Services.AddScoped<IRateLimitService, FakeRateLimitService>();
builder.Services.AddScoped<IRateLimitValidator, RateLimitValidator>();
builder.Services.AddScoped<IRateLimitValidationQueryProvider, RateLimitValidationQueryProvider>();

var app = builder.Build();
app.UseRateLimitMiddleware();
app.UseAuthorization();
app.MapControllers();
app.Run();