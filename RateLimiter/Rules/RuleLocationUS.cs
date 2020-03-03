using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RateLimiter.Model;
using RuleEngine;

namespace RateLimiter.Rules
{
    public class RuleLocationUS : IRule
    {
        public string Name => "RuleLocationUS";

        public void Execute(IRuleEnvironment environment)
        {
            var result = environment.GetFact<RuleResult>("result");
            if (!result.Allow)
                return;
            TokenInfo tokenInfo = environment.GetFact<TokenInfo>("tokenInfo");

            if (tokenInfo.Location.ToLower() != "us")
            {
                result.Allow = false;
                result.Message = "Requests not from US.";
            }
        }
    }
}
