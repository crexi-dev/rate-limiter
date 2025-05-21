using RateLimiter.Api.Middleware;
using RateLimiter.Core.Configuration;
using RateLimiter.Infrastructure.DependencyInjection;

// Enhanced Program.cs with fixed hybrid rate limiting

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Use ENHANCED hybrid rate limiting with proper configuration precedence
builder.Services.AddEnhancedHybridRateLimiting(
    options =>
    {
        options.EnableRateLimiting = true;
        options.IncludeHeaders = true;
        options.HeaderPrefix = "X-RateLimit";
        options.StatusCode = 429;
        options.ClientIdHeaderName = "X-ClientId";
        options.RegionHeaderName = "X-Region";
    },
    configRules =>
    {
        var rateLimitingSection = builder.Configuration.GetSection(EnhancedRateLimitConfiguration.SectionName);
        if (rateLimitingSection.Exists())
        {
            rateLimitingSection.Bind(configRules);
        }
    });

// Add resource-based key builder
builder.Services.AddResourceBasedKeyBuilder();

// Add Redis rate limiting if configured
var redisConnectionString = builder.Configuration["Redis:ConnectionString"];
if (!string.IsNullOrEmpty(redisConnectionString))
{
    builder.Services.AddRedisRateLimiting(redisConnectionString);
}

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<RateLimitingMiddleware>();
app.UseAuthorization();
app.MapControllers();

app.Run();

// Make Program class public for testing
public partial class Program { }
