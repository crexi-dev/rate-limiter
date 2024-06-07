namespace RateLimiter.Repositories.Interfaces;

public interface IRateLimitRuleRepository
{
    RequestsPerTimespanRuleOptions RequestsPerTimespanRule();
    RequestsPerPeriodRuleOptions RequestsPerPeriodRule();
}