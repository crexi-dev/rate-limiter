using Microsoft.Extensions.DependencyInjection;
using RateLimiter.RulesEngine.Interfaces;
using RateLimiter.RulesEngine.Rules;
using RateLimiter.RulesEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRateLimitServices(this IServiceCollection services)
        {
            services.AddSingleton<IRuleEngine, RuleEngine>();
            services.AddSingleton<IRateLimiterRule, RequestsPerTimeRule>();
            services.AddSingleton<IRateLimiterRule, TimeRule>();

            return services;
        }
    }
}
