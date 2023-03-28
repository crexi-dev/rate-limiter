using System.Collections.Generic;
using RateLimiter.Interfaces;
using RateLimiter.RequestMathers;

namespace RateLimiter;

public class MatchersFactory
{
    public static IMatcher CreateMatchers(RateLimitConfiguration option)
    {
        IMatcher result = new TokenMatcher();
        if (!string.IsNullOrEmpty(option.Url))
        {
            result = result.Combine(new UrlMatcher(option.Url));
        }
        
        if (!string.IsNullOrEmpty(option.HttpMethod))
        {
            var http = new HttpMethodMatcher(option.HttpMethod);
            result = result.Combine(http);
        }
        
        if (!string.IsNullOrEmpty(option.Regions))
        {
            var region = new RegionMather(option.Regions);
            result = result.Combine(region);
        }

        return result;
    }
}