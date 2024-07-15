using Microsoft.AspNetCore.Http;
using RateLimiter.Interfaces.Business.Rules;
using RateLimiter.Interfaces.Models;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Business.Rules
{
    public abstract class BaseRateLimitRule : IRateLimitRule
    {
        public abstract Task<IRateLimitRuleResult> Verify(HttpContext context, IUser user,
            IEndpoint endpoint, DateTime requestTime);

        protected string ComputeHash(IUser user, IEndpoint endpoint)
        {
            return Hash($"{user.Id}{endpoint.PathPattern}{(int)endpoint.Verbs}{GetType().Name}");
        }

        private string Hash(string value)
        {
            return Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
        }
    }
}
