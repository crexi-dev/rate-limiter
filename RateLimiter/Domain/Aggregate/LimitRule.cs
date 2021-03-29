using RateLimiter.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Domain.Aggregate
{
    public class LimitRule : ValueObject
    {
        public int TimeSpan { get; set; }

        public int Threshold { get; set; }

        public string GeoLocation { get; set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return TimeSpan;
            yield return Threshold;
            yield return GeoLocation;
        }
    }
}
