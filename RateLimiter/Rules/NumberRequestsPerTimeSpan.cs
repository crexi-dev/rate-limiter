using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RateLimiter.Rules
{
    public class NumberRequestsPerTimeSpan : Rule
    {
        public override async Task<bool> ValidateAsync(IEnumerable<DateTime> requestDates)
        {
            var dateNow = DateTime.UtcNow;

            int requestCount = requestDates.Count(x => dateNow - x <= Period);

            return await Task.FromResult(!(requestCount > Limit));
        }
    }
}
