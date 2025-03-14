using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Rules
{
    public class EmptyRule(IRule rule = null, ICacheHelper cacheHelper = null) : AbstractRule(rule, cacheHelper)
    {
        protected override bool CheckRuleLimit()
        {
            return true;
        }
    }
}
