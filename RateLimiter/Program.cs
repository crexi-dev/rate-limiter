
using RateLimiter;
using RateLimiter.Middleware;
using RateLimiter.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDistributedMemoryCache(options =>
{

});

builder.Services.AddSingleton<ILogApiHitCountService, LogApiHitCountService>();

builder.Services.AddSingleton<IRateLimitRules, RateLimitRules>();

var app = builder.Build();

app.UseMiddleware<RateLimitMiddleware>();
app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

app.Run();
