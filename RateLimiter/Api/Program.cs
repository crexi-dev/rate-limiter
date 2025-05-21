using RateLimiter.Api.Middleware;
using RateLimiter.Core.Configuration;
using RateLimiter.Infrastructure.DependencyInjection;

// Make the Program class public for testing
public partial class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Check if enhanced rate limiting is configured
        var useEnhancedRateLimiting = !string.IsNullOrEmpty(builder.Configuration["JwtAuthentication:SecretKey"]) ||
                                     !string.IsNullOrEmpty(builder.Configuration["GeoIP:DatabasePath"]);

        if (useEnhancedRateLimiting)
        {
            // Configure enhanced hybrid rate limiting with authentication and GeoIP
            builder.Services.AddFullHybridRateLimiting(
                rateLimitOptions =>
                {
                    rateLimitOptions.EnableRateLimiting = true;
                    rateLimitOptions.IncludeHeaders = true;
                    rateLimitOptions.HeaderPrefix = "X-RateLimit";
                    rateLimitOptions.StatusCode = 429;
                    rateLimitOptions.ClientIdHeaderName = "X-ClientId";
                    rateLimitOptions.RegionHeaderName = "X-Region";
                },
                configRules =>
                {
                    // Configuration will be read from appsettings.json RateLimiting section
                    var rateLimitingSection = builder.Configuration.GetSection(EnhancedRateLimitConfiguration.SectionName);
                    if (rateLimitingSection.Exists())
                    {
                        rateLimitingSection.Bind(configRules);
                    }
                },
                jwtOptions =>
                {
                    jwtOptions.SecretKey = builder.Configuration["JwtAuthentication:SecretKey"] ?? "";
                    jwtOptions.Issuer = builder.Configuration["JwtAuthentication:Issuer"];
                    jwtOptions.Audience = builder.Configuration["JwtAuthentication:Audience"];
                    jwtOptions.Enabled = !string.IsNullOrEmpty(jwtOptions.SecretKey);
                },
                geoOptions =>
                {
                    geoOptions.DatabasePath = builder.Configuration["GeoIP:DatabasePath"];
                    geoOptions.DefaultRegion = builder.Configuration["GeoIP:DefaultRegion"] ?? "UNKNOWN";
                    geoOptions.Enabled = !string.IsNullOrEmpty(geoOptions.DatabasePath);
                });
        }
        else
        {
            // Use hybrid rate limiting (without enhanced auth/geo features)
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
                    // Configuration will be read from appsettings.json RateLimiting section
                    var rateLimitingSection = builder.Configuration.GetSection(EnhancedRateLimitConfiguration.SectionName);
                    if (rateLimitingSection.Exists())
                    {
                        rateLimitingSection.Bind(configRules);
                    }
                });
        }

        // Add resource-based key builder
        builder.Services.AddResourceBasedKeyBuilder();

        // Add Redis rate limiting if configured
        var redisConnectionString = builder.Configuration["Redis:ConnectionString"];
        if (!string.IsNullOrEmpty(redisConnectionString))
        {
            builder.Services.AddRedisRateLimiting(redisConnectionString);
        }

        // Check if we're in a test environment
        var isTestEnv = builder.Environment.EnvironmentName == "Testing" || 
                        AppDomain.CurrentDomain.FriendlyName.Contains("testhost") ||
                        AppDomain.CurrentDomain.FriendlyName.Contains("test");
        
        if (isTestEnv)
        {
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();
        }

        var app = builder.Build();

        // Configure the HTTP request pipeline
        if (app.Environment.IsDevelopment() || isTestEnv)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseMiddleware<RateLimitingMiddleware>();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}
