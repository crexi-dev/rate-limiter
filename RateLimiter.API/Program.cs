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