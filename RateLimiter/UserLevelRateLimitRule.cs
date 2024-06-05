using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
    public class UserLevelRateLimitRule : RateLimitRule
    {
        private readonly ConcurrentDictionary<string, RateLimitRule> _userLevelRules = new();
        private readonly Func<string, string> _getUserLevel;

        public UserLevelRateLimitRule(Func<string, string> getUserLevel)
        {
            _getUserLevel = getUserLevel;
        }

        public void ConfigureUserLevel(string userLevel, RateLimitRule rule)
        {
            _userLevelRules[userLevel] = rule;
        }

        public override bool AllowRequest(string clientId)
        {
            var userLevel = _getUserLevel(clientId);
            return _userLevelRules.ContainsKey(userLevel) && _userLevelRules[userLevel].AllowRequest(clientId);
        }
    }
}
