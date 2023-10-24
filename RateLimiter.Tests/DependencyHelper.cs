using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using RateLimiter.Contexts;
using RateLimiter.Extensions;
using RateLimiter.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Tests
{
    internal static class DependencyHelper
    {
        private static IServiceProvider _provider;
        private static IConfigurationRoot _configuration;

        private static IConfigurationRoot GetIConfigurationRoot(bool reset)
        {
            if (_configuration != null && !reset)
                return _configuration;

            _configuration = new ConfigurationBuilder()
                .SetBasePath(TestContext.CurrentContext.TestDirectory)
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
            return _configuration;
        }

        internal static IServiceProvider Provider(bool reset)
        {
            if (_provider != null && !reset)
                return _provider;

            var services = new ServiceCollection();
            var config = GetIConfigurationRoot(reset);
            services.ConfigureRateLimiter(config);
            services.AddDbContext<RateDBContextBase>(db => db.UseInMemoryDatabase(Guid.NewGuid().ToString()));

            _provider = services.BuildServiceProvider();
            return _provider;
        }

        internal static T GetRequiredService<T>(bool reset)
        {
            var prov = Provider(reset);

            return prov.GetRequiredService<T>();
        }
    } 
}
