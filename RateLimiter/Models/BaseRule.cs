using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RateLimiter.Enums;

namespace RateLimiter.Models
{
    public abstract class BaseRule
    {
        private readonly RuleTypes _ruleType;

        protected BaseRule(RuleTypes ruleType)
        {
            _ruleType = ruleType;
        }
    }
}
