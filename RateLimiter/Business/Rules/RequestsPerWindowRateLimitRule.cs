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
    public class RequestsPerWindowRateLimitRule : BaseRateLimitRule
    {
        private readonly uint _requestLimit;
        private readonly uint _windowMs;

        private readonly IRateLimitRepository _repository;

        private const string RequestCountKey = "RequestCount";
        private const string WindowStartTimeKey = "WindowStartTime";
        private const string WindowMsKey = "WindowMs";
        private const string RequestLimitKey = "RequestLimit";

        public RequestsPerWindowRateLimitRule(IRateLimitRepository repository, IDictionary<string, object> parameters)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));

            _requestLimit = parameters.ToUInt(RequestLimitKey);

            _windowMs = parameters.ToUInt(WindowMsKey);
        }

        public override async Task<IRateLimitRuleResult> Verify(HttpContext context, IUser user, IEndpoint endpoint, DateTime requestTime)
        {
            var repositoryKey = ComputeHash(user, endpoint);

            var data = await _repository.Retrieve(repositoryKey);

            // First request
            if (data == null)
            {
                data = new Dictionary<string, object>();
                data[RequestCountKey] = 0u;
                data[WindowStartTimeKey] = requestTime;
            }

            // Check for/reset the limit if we're outside the window
            if (requestTime.Subtract((DateTime)data[WindowStartTimeKey]).TotalMilliseconds >= _windowMs)
            {
                data[RequestCountKey] = 0u;
                data[WindowStartTimeKey] = requestTime;
            }

            var count = (uint)data[RequestCountKey];

            var verified = count + 1 <= _requestLimit;

            data[RequestCountKey] = count + 1;

            await _repository.Update(repositoryKey, data);

            var error = verified
                ? null
                : $"Request limit of {_requestLimit} exceeded in {_windowMs}ms window starting at {data[WindowStartTimeKey]}";

            return new RateLimitRuleResult(verified, error);
        }
    }
}
