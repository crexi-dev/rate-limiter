using RateLimiter.Application.Interfaces;
using RateLimiter.Domain.Aggregate;
using RateLimiter.Domain.Contexts;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.Infastructure.Filters
{
    public class GeoLocationRuleFilter : ILimitRuleFilter
    {
        public IEnumerable<LimitRule> FilterByRequest(Resource resource, RequestContext context)
        {
                return resource.RateLimitRules.Any(r => r.GeoLocation == context.ClientContext.GeoLocation) ?
                        resource.RateLimitRules.Where(r => r.GeoLocation == context.ClientContext.GeoLocation) :
                        resource.RateLimitRules.Where(r => r.GeoLocation == null);
        }
    }
}
