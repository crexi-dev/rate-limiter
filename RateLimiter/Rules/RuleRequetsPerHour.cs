using RuleEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Rules
{
    class RuleRequetsPerHour : IRule
    {
        public string Name => "RuleRequetsPerHour";

        private const int hitLimit = 100;

        public void Execute(IRuleEnvironment environment)
        {
            var result = environment.GetFact<RuleResult>("result");
            if (!result.Allow)
                return;

            if (environment.GetFact<int>("requestThisHour") > hitLimit)
            {
                result.Allow = false;
                result.Message = "X requests per hour";
            }
        }
    }
}
