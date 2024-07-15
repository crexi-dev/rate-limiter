using System.Collections.Generic;

namespace RateLimiter;

// extendable via adding new extension methods
// NOTE: public would actually be internal
public static class RateLimiterRules
{
    public static RateLimiterState JWTIngest(RateLimiterState? state)
    { 
        bool jwtValidationResult = true; 

        bool updatedIsAlloweable = state?.IsAlloweable ?? true;
        return new RateLimiterState(updatedIsAlloweable && jwtValidationResult);
    }

    public static RateLimiterState IsTimeSinceLastCallAllowable(this RateLimiterState? state, RateLimiterConfig rules, int timeSinceLastCall)
    {
        bool result = timeSinceLastCall <= rules.TimeSinceLastCall;

        bool updatedIsAlloweable = state?.IsAlloweable ?? true;
        return new RateLimiterState(updatedIsAlloweable && result);
    }

    public static RateLimiterState IsXRequestPerTimespanAllowable(this RateLimiterState? state, RateLimiterConfig rules, int noOfRequests)
    {
        bool result = noOfRequests <= rules.XRequestsPerTimespan;

        bool updatedIsAlloweable = state?.IsAlloweable ?? true;
        return new RateLimiterState(updatedIsAlloweable && result);
    }
}

