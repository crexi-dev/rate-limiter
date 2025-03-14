using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Rules
{
    public class RequestsLimitRule : AbstractRule
    {
        private TimeSpan _timeLimit;
        private int _requestsCount;
        private string _id;

        public RequestsLimitRule(IRule rule, Dictionary<string, string> Variables, ICacheHelper cacheHelper, string id) :base (rule, cacheHelper) 
        {
            _timeLimit = TimeSpan.FromMilliseconds(int.Parse(Variables["time"]));
            _requestsCount = int.Parse(Variables["requests"]);
        }

        protected override bool CheckRuleLimit()
        {
            return _cacheHelper.RequestsCount(_id, _timeLimit)>= _requestsCount;
        }
    }
}
