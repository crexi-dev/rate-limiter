using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using RateLimiter.API.Controllers;
using RateLimiter.Implementation;

namespace RateLimiter.API;

public class Program
{
    public static void Main(string[] args)
    {
        var hostBuilder = new HostBuilder()
            .ConfigureHostConfiguration(configHost =>
            {
                configHost.SetBasePath(Directory.GetCurrentDirectory());
                configHost.AddJsonFile("appsettings.json", optional: true);
                configHost.AddEnvironmentVariables(prefix: "ASPNETCORE_");
                configHost.AddCommandLine(args);
            })
            .ConfigureServices((hostContext, services) =>
            {
                var rateLimitConfig = new RateLimitConfig();
                hostContext.Configuration.GetSection("RateLimiting").Bind(rateLimitConfig);

                foreach (var ruleConfig in rateLimitConfig.Rules)
                {
                    var ruleType = Type.GetType(ruleConfig.RuleType);
                    if (ruleType == null) continue;

                    var rule = (IRateLimitingRule)Activator.CreateInstance(ruleType, ruleConfig.Parameters)!;
                    services.AddSingleton(typeof(IRateLimitingRule), rule);
                }

                services.AddSingleton(rateLimitConfig);
            });

        var host = hostBuilder.Build();
        host.Run();
    }
}