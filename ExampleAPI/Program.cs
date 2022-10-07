using API.Extensions;
using API.Middlewares;

namespace API
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder
                .Configuration
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("appsettings.json", false)
                .Build();
            
            builder
                .Services
                .AddApiServices();
            
            var app = builder.Build();

            app.UseMiddleware<RequestLimiterMiddleware>();

            app.MapGet("/", () => "ok");

            await app.RunAsync();
        }
    }
}