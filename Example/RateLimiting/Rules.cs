using Example.RateLimiting.ContextProviders;
using RateLimiter.Implementations;
using RateLimiter.Rules;

namespace Example.RateLimiting;

public class DefaultUSRateLimitRule() :
    XRequestsPerTimeSpanRuleAttribute<USContextProvider>(100, "00:01:00");

public class DefaultEURateLimitRule() :
    XRequestsPerTimeSpanRuleAttribute<USContextProvider>(1, "00:00:10");
    
public class SomeSpecialRateLimitRule()
    : XRequestsPerTimeSpanRuleAttribute<EmptyContextProvider>(200, "01:00:00");