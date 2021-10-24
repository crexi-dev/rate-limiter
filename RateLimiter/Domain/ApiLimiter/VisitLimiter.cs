using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Domain.ApiLimiter
{
    public class VisitLimiter
    {
        public string Token { get; set; }

        private Dictionary<(string Resource, string Region), List<IRule>> Rules;

        public VisitLimiter AddRule(string Resource, string Region, IRule rule)
        {

        }
    }
}
