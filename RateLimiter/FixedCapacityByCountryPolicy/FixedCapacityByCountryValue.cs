using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("RateLimiter.Tests")]
namespace RateLimiter.FixedCapacityByCountryPolicy
{
    internal class FixedCapacityByCountryValue
    {
        public IDictionary<string, IEnumerable<DateTime>> Calls { get; set; }
        public FixedCapacityByCountryValue()
        {
            Calls = new Dictionary<string, IEnumerable<DateTime>>();
        }
    }
}
