using RateLimiter.Data;
using RateLimiter.Model.Rule;
using RateLimiter.Model.Rule.Config;
using RateLimiter.Rule.Validator;
using System;

namespace RateLimiter.Rule
{
    internal class ResourceRuleFactory
    {
        public static ValidatorBase Get(IRepositoryRateLimiter repo, RuleConfigBase config)
        {
            var configMismatchMessage = $"Configuration Mismatch for rule type {config.Rule}";
            switch (config.Rule)
            {
                case EnumRateLimitRule.EnabledResource:
                    return (config is RuleConfigEnabledResource enabledResource) ?
                        new ValidatorEnabledResource(repo, enabledResource) : throw new ArgumentException(configMismatchMessage);                    
                case EnumRateLimitRule.TimespanThreshold:
                    return (config is RuleConfigTimespanThreshold timespanConfig) ?
                        new ValidatorTimespanThreshold(repo, timespanConfig) : throw new ArgumentException(configMismatchMessage);                    
                case EnumRateLimitRule.IpWhiteList:
                    return (config is RuleConfigIpWhiteList whiteListConfig) ?
                        new ValidatorIpWhiteList(repo, whiteListConfig) : throw new ArgumentException(configMismatchMessage);                    
                case EnumRateLimitRule.IpBlackList:
                    return (config is RuleConfigIpBlackList blackListConfig) ?
                        new ValidatorIpBlackList(repo, blackListConfig) : throw new ArgumentException(configMismatchMessage);                    
                default:
                    throw new NotImplementedException($"Implementation required for resource rule: {config.Rule}");
            }
        }        
    }
}
