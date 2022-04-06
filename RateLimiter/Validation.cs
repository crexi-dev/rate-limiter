using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter
{
    public class Validation
    {
        private readonly List<ILimitRule> _limitRules;

        public Validation(List<ILimitRule> limitRules)
        {
            _limitRules = limitRules;
        }

        public bool Validate(string ruleName, string resource, string identifer)
        {
            try
            {
                var rule = _limitRules.Single(x => x.Name == ruleName);

                return rule.Validate(resource, identifer);
            }
            catch (Exception ex)
            {
                // TODO: Use real logger framework.
                Console.WriteLine(ex.Message);

                throw;
            }
        }

    }
}
