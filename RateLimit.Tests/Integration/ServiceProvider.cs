using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RateLimit.Tests.Integration
{
    internal static class ServiceProviderHelper
    {
        private static IServiceProvider Provider()
        {
            var services = new ServiceCollection();
            var config = new ConfigurationBuilder().AddJsonFile("TestConfig.json", optional: true, reloadOnChange: true).Build();
            services.AddRateRuleAService(config);
            services.AddRateRuleBService(config);
            services.AddRateRuleCService(config);
            return services.BuildServiceProvider();
        }

        public static IEnumerable<T> GetServices<T>()
        {
            var serviceProvider = Provider();
            return serviceProvider.GetServices<T>();
        }
        public static T GetRequiredService<T>()
        {
            var serviceProvider = Provider();
            return serviceProvider.GetRequiredService<T>();
        }

    }
}
