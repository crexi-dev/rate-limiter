using System.Collections.Generic;

namespace RateLimiter.FixedCapacityByCountryPolicy
{
    public record class FixedCapacityByCountryMiddleWareOptions
    {
        public List<FixedCapacityByCountryItemMiddleWareOptions> Items { get; set; }
        public FixedCapacityByCountryMiddleWareOptions()
        {
            Items = [];
        }
    }
}
