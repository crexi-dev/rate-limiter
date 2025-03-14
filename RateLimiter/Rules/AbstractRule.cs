using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Rules
{
    public abstract class AbstractRule : IRule
    {
        private IRule _rule;
        protected ICacheHelper _cacheHelper;

        public AbstractRule(IRule rule, ICacheHelper cacheHelper)
        {
            _rule = rule;
            _cacheHelper = cacheHelper;
        }

        protected abstract bool CheckRuleLimit();

        public bool CheckLimit()
        {
            return CheckRuleLimit() & (_rule?.CheckLimit() ?? true);
        }

    }

}