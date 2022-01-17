using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter
{
    public class RateLimiter
    {
        private ClientLogsStorage storage;

        private List<IRule> ruleList;

        public RateLimiter(List<IRule> rules)
        {
            ruleList = rules;
            storage = new ClientLogsStorage();
        }

        public bool Request(string token)
        {
            foreach (var rule in ruleList)
            {
                if(!rule.CanAccess(token, storage))
                {
                    return false;
                }
            }
            storage.AddLog(token);
            return true;
        }

    }
}
