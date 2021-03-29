using RateLimiter.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Domain.ValueObjects
{
    public class VisitContext : ValueObject
    {
        public long WindowStart { get; set; }
        public int Counter { get; set; }
        public long LastAccess { get; set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return WindowStart;
            yield return Counter;
            yield return LastAccess;
        }

        public VisitContext Clone()
        {
            return new VisitContext
            {
                Counter = Counter,
                LastAccess = LastAccess,
                WindowStart = WindowStart
            };
        }
    }
}
