using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RateLimiter.BL;
using RateLimiter.BL.Services;
using RateLimiter.BL.ServicesInterfaces;
using RateLimiter.Filters;
using System;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddRouting(options => options.LowercaseUrls = true);
services.AddControllers(config =>
{
    config.Filters.Add(new AuthorizationFilter());
});

services.AddDistributedMemoryCache();

services.AddScoped<IHomeService, HomeService>();
services.AddScoped<RateLimiterFilter>();
services.AddScoped<LogRequestsFilter>();

services.AddSingleton<IRequestRepository, RequestRepository>();

services.AddScoped<IRaceLimiterRuleService, DebounceRuleService>();
services.AddScoped<IRaceLimiterRuleService, RequestPerTimespanRuleService>();
services.AddScoped<IRaceLimiterService, RaceLimiterService>();


var app = builder.Build();


app.UseRouting();


app.UseHttpsRedirection();


app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();

//For Tests
public partial class Program { }