using RuleEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Rules
{
    class RuleTimespanPassedSinceLastCall : IRule
    {
        public string Name => "RuleTimespanPassedSinceLastCall";

        public void Execute(IRuleEnvironment environment)
        {
            throw new NotImplementedException();
        }
    }
}
