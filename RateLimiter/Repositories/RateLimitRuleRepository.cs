using Microsoft.Extensions.Options;
using RateLimiter.Repositories.Interfaces;

namespace RateLimiter.Repositories;

public class RateLimitRuleRepository(IOptions<ApplicationSettings> settings)
    : IRateLimitRuleRepository
{
    public RequestsPerTimespanRuleOptions RequestsPerTimespanRule()
    {
        return settings.Value.RequestsPerTimespanRules;
    }

    public RequestsPerPeriodRuleOptions RequestsPerPeriodRule()
    {
        return settings.Value.RequestsPerPeriodRules;
    }
}