using RateLimiter.Domain.Aggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Domain.Contexts
{
    public class EvaluationContext
    {
        public RequestContext RequestContext { get; set; }

        public IEnumerable<LimitRule> RuleSet { get; set; }
    }
}
