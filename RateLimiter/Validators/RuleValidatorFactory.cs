using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.Validators
{
    public class RuleValidatorFactory : IRuleValidatorFactory
    {
        private IEnumerable<IRuleValidator> _ruleValidators;

        public RuleValidatorFactory(IEnumerable<IRuleValidator> ruleValidators)
        {
            _ruleValidators = ruleValidators;
        }

        public IRuleValidator GetValidator(int ruleId)
        {
            var ruleValidator = _ruleValidators.First(x => x.RuleId == ruleId);
            return ruleValidator;
        }
    }
}
