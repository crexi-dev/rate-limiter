using Microsoft.AspNetCore.Http;
using RateLimiter.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
    public class RulesFactory : IRulesFactory
    {
        private IConfigLoader _configLoader;
        private ICacheHelper _cacheHelper;

        public RulesFactory(IConfigLoader configLoader, ICacheHelper cacheHelper)
        {
            _configLoader = configLoader;
            _cacheHelper = cacheHelper;
        }

        public IRule GetRule(ContextDto context)
        {            
            var rules = _configLoader.LoadRules(context);
            IRule tempRule = new EmptyRule();

            foreach (var rule in rules)
            {
                switch (rule.Name ?? string.Empty)
                {
                    case "RequestsLimit":
                        tempRule = new RequestsLimitRule(tempRule, rule.Variables, _cacheHelper, context.Id);
                        break;
                    case "TimeLimit":
                        tempRule = new TimeLimitRule(tempRule, rule.Variables, _cacheHelper, context.Id);
                        break;
                    default:
                        tempRule = new EmptyRule(tempRule);
                        break;
                }
            }
            return tempRule;
        }
    }
}
