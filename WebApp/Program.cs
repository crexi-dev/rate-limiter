
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RateLimiter;
using RateLimiter.Repositories;
using RateLimiter.Repositories.Interfaces;
using RateLimiter.Services;
using RateLimiter.Services.Interfaces;

namespace WebApp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.Configure<ApplicationSettings>(builder.Configuration.GetSection("Application"));
        builder.Services.AddScoped<IRateLimitRuleRepository, RateLimitRuleRepository>();
        builder.Services.AddScoped<IRateLimitRepository, RateLimitRepository>();
        builder.Services.AddScoped<RequestsPerTimespanValidation>();
        builder.Services.AddScoped<RequestsPerPeriodValidation>();
        builder.Services.AddScoped<IRequestValidator>(provider =>
        {
            var timespanValidator = provider.GetRequiredService<RequestsPerTimespanValidation>();
            var periodValidator = provider.GetRequiredService<RequestsPerPeriodValidation>();
            timespanValidator.SetNext(periodValidator);
            return timespanValidator;
        });

        builder.Services.AddScoped<IRateLimitingManager, RateLimitingManager>();

        builder.Services.AddMemoryCache();

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}