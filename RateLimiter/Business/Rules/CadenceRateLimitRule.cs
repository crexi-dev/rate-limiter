using Microsoft.AspNetCore.Http;
using RateLimiter.Extensions;
using RateLimiter.Interfaces.DataAccess;
using RateLimiter.Interfaces.Models;
using RateLimiter.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RateLimiter.Business.Rules
{
    public class CadenceRateLimitRule : BaseRateLimitRule
    {
        private readonly IRateLimitRepository _repository;

        private readonly uint _minimumDelayBetweenRequestsMs;

        private const string LastAccessedKey = "LastAccessed";
        private const string MinimumDelayBetweenRequestsMsKey = "MinimumDelayBetweenRequestsMs";

        public CadenceRateLimitRule(IRateLimitRepository repository, IDictionary<string, object> parameters)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));

            _minimumDelayBetweenRequestsMs = parameters.ToUInt(MinimumDelayBetweenRequestsMsKey);
        }

        public override async Task<IRateLimitRuleResult> Verify(HttpContext context, IUser user,
            IEndpoint endpoint, DateTime requestTime)
        {
            var verified = true;

            var repositoryKey = ComputeHash(user, endpoint);

            var data = await _repository.Retrieve(repositoryKey);

            if (data == null)
            {
                data = new Dictionary<string, object>();
            }
            else
            {
                var last = (DateTime)data[LastAccessedKey];

                var acceptable = last.AddMilliseconds(_minimumDelayBetweenRequestsMs);

                verified = requestTime >= acceptable;
            }

            data[LastAccessedKey] = requestTime;

            await _repository.Update(repositoryKey, data);

            var error = verified
                ? null
                : $"Request time of {requestTime} arrive sooner than {data[LastAccessedKey]} +{_minimumDelayBetweenRequestsMs}ms";

            return new RateLimitRuleResult(verified, error);
        }
    }
}
