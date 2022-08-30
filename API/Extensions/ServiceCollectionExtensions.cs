using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using RateLimiter.Extensions;
using RateLimiter.InMemoryCache.Options;
using RateLimiter.Models.Enums;
using RateLimiter.Models.Rules;
using RateLimiter.Options;

namespace API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddApiServices(this IServiceCollection services)
        {
            services.AddRateLimiterServices();
            AddRateLimiterOptions(services);
            AddInMemoryCacheOptions(services);
        }

        private static void AddRateLimiterOptions(IServiceCollection services)
        {
            services.AddOptions<RateLimiterOptions>()
                .Configure(x =>
                {
                    x.RateLimiterRules = new Dictionary<RateLimiterType, RateLimiterRuleBase>
                    {
                        {
                            RateLimiterType.XRequestsPerTimespan, new XRequestsPerTimespanRateLimiterRule
                            {
                                RequestsLimit = 1,
                                TimeSpanPeriod = TimeSpan.FromSeconds(20)
                            }
                        },
                        {
                            RateLimiterType.CertainTimespanPassedSinceTheLastCall,
                            new CertainTimespanPassedSinceTheLastCallRateLimiterRule()
                            {
                                TimeSpanPeriod = TimeSpan.FromSeconds(20)
                            }
                        }
                    };
                });
        }

        private static void AddInMemoryCacheOptions(IServiceCollection services)
        {
            services.AddOptions<InMemoryCacheOptions>()
                .Configure(x =>
                {
                    x.ExpirationTimeSpan = TimeSpan.FromMinutes(5);
                });
        }
    }
}
