using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace RateLimiter
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger logger = factory.CreateLogger<Program>();
            logger.LogInformation("Hi! Starting up Rate-Limiter");

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}