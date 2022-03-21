using Microsoft.OpenApi.Models;
using RateLimiter.API.CounterKeyBuilder;
using RateLimiter.API.Filters;
using RateLimiter.API.Middlewares;
using RateLimiter.API.Processors;
using RateLimiter.API.Resolvers;
using RateLimiter.API.Store;
using RateLimiter.Core.Models.RateLimit;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.OperationFilter<CustomHeaderSwaggerAttribute>();
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Rate Limit API",
        Description = @"Please use these tokens <br /> "
                      + "EU - 7ma0e8+yl37aCqJIK9vLcwna+sSdXzvg9PVsc0zhjTKSj3GILY/GAVRIdYBGSAbqRAYRBikxvm0FOIo+BQNPcw== <br />"
                      + "US - 8hp8M1z31XxXtbmyaRXJnlv0/10ziScLf3Xp7EBTWbnRe7RWyNgySvIiuaje2rYTeMvuLAPRODlrS0K3e/NbsA==",
    });
});

builder.Services.AddMemoryCache();

builder.Services.AddSingleton<IIdentityResolver, IdentityResolver>();
builder.Services.AddSingleton<IRateLimitCounterStore, RateLimitCounterStore>();
builder.Services.AddSingleton<IClientCounterKeyBuilder, ClientCounterKeyBuilder>();
builder.Services.AddSingleton<IRateLimitProcessor, RateLimitProcessor>();

IConfigurationSection customerClientSettingsConfig = builder.Configuration.GetSection("RateLimiting");
builder.Services.Configure<RateLimitOptions>(customerClientSettingsConfig);

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<RateLimitMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();