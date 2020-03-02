using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Rules
{
    public class RuleResult
    {
        public bool Allow { get; set; }
        public string RuleName { get; set; }
        public string Message { get; set; }
    }
}
