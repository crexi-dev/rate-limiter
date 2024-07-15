using Microsoft.AspNetCore.Http;
using RateLimiter.Interfaces.Business.Rules;
using RateLimiter.Interfaces.Models;
using RateLimiter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RateLimiter.Tests.CustomRules
{
    /// <summary>
    /// Demo implementation of a custom rule
    /// </summary>
    public class BlacklistRule : IRateLimitRule
    {
        private const string DeniedUsersIdsListKey = "DeniedUsersIdsList";

        private readonly string[] _deniedUsers;

        public BlacklistRule(IDictionary<string, object> parameters)
        {
            _deniedUsers = ((string)parameters[DeniedUsersIdsListKey])
                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                .ToArray();
        }

        public async Task<IRateLimitRuleResult> Verify(HttpContext context, IUser user, IEndpoint endpoint, DateTime requestTime)
        {
            var proceed = !_deniedUsers.Contains(user.Id);

            var error = proceed ? null : $"User {user.Id} is denied";

            var result = new RateLimitRuleResult(proceed, error);

            return result;
        }
    }
}
