using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RateLimiter.Rules
{
    public class PeriodBetweenRequests : Rule
    {
        public override async Task<bool> ValidateAsync(IEnumerable<DateTime> requestDates)
        {
            var dateNow = DateTime.UtcNow;

            requestDates = requestDates.OrderByDescending(x => x);
            var lastReqDate = requestDates.FirstOrDefault();

            if (lastReqDate == null)
            {
                return true;
            }

            return await Task.FromResult(!(dateNow - lastReqDate <= Period));
        }
    }
}
