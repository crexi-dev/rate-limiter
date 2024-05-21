using RateLimiter.Nugget.DependencyInjection;
using RateLimiter.RateLimitingRules;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddRateLimiting<RequestsPerTimespanRule>();
builder.Services.AddRateLimitingRules(new RequestsPerTimespanRule());

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseRateLimiterMiddleware<RequestsPerTimespanRule>();

app.MapControllers();

app.Run();
