using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("RateLimiter.Tests")]
namespace RateLimiter.FixedCapaicityPolicy
{
    internal struct FixedCapacityValue
    {
        public DateTime LastRequest { get; set; }
    }
}
