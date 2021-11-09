using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using RateLimiter.Application.AccessRestriction.Rule.RateLimit;
using RateLimiter.Application.MapperProfiles;
using RateLimiter.Application.Services;
using RateLimiter.Data.Repository;

namespace RateLimiter.Test
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IRateRuleEngineService, RateRuleEngineService>();
            services.AddScoped<IRuleRepository, RuleRepository>();
            services.AddScoped<IResourceRepository, ResourceRepository>();
            services.AddScoped<IResourceAccessRepository, ResourceAccessRepository>();
            services.AddScoped<IResourceService, ResourceService>();
            services.AddTransient<IPerMinuteRule, PerMinuteRule>();
            services.AddTransient<ITimeElapsedRule, TimeElapsedRule>();
            services.AddAutoMapper(typeof(DomainToViewModelProfile));
        }
    }
}
