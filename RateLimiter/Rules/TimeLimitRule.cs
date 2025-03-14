using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Rules
{
    public class TimeLimitRule : AbstractRule
    {
        private TimeSpan _timeLimit;
        private string _id;

        public TimeLimitRule(IRule rule, Dictionary<string, string> Variables, ICacheHelper cacheHelper, string id) : base(rule, cacheHelper)
        {
            _timeLimit = TimeSpan.FromMilliseconds(int.Parse(Variables["time"]));
        }

        override protected bool CheckRuleLimit()
        {
            return _cacheHelper.LastRequestTime(_id) < _timeLimit;
        }
    }
}
