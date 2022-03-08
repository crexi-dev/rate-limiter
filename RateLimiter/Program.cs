using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

// Api Call Simulation
namespace RateLimiter
{
    class Program
    {   
        static void Main(string[] args)
        {
            // Create service collection
            var services = new ServiceCollection();
            ConfigureServices(services);

            ServiceProvider serviceProvider = services.BuildServiceProvider();
            ApiSimulator apiSimulator = serviceProvider.GetService<ApiSimulator>() ?? throw new ArgumentNullException(nameof(serviceProvider));

            do
            {
                Console.WriteLine($"Enter an api resource to call (ApiResource1, ApiResource2, ApiResource3). To exit just press enter.");
                string? input = Console.ReadLine();
                if (string.IsNullOrEmpty(input)) break;

                apiSimulator.SimulateApiCall(input);
            } while (true);
            return;
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // Build config object and get rules from settings defined in apsettings.json
            var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

            // Dependency Injection
            services.Configure<Settings>(config.GetSection("Settings"));
            services.AddSingleton<IRateLimitLibrary, RateLimitLibrary>();
            services.AddSingleton<ApiSimulator>();

        }
    }
}
