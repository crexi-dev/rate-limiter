using Microsoft.Extensions.DependencyInjection;
using RateLimiter.ServiceConfiguratoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Tests.Rules_Integration
{
    internal static class SingletonProvider
    {
        private static IServiceProvider _serviceProvider;
        private static readonly object padlock = new object();

        public static IServiceProvider ServiceProvider
        {
            get
            {
                if (_serviceProvider == null)
                {
                    lock (padlock)
                    {
                        if (_serviceProvider == null)
                        {
                            var serviceCollection = new ServiceCollection();
                            serviceCollection.AddMemoryCache();
                            serviceCollection.BuildServiceProvider();

                            _serviceProvider = serviceCollection.BuildServiceProvider();
                        }
                    }
                }

                return _serviceProvider;
            }
        }
    }
}
