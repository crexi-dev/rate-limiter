using RateLimiter.State;
using System;

namespace RateLimiter.Rules
{
    public class RequestElapsedTime : IRule<RequestInfo>
    {
        private readonly IRulePersist _rulePersist;
        private readonly TimeSpan _elapsedConfig;

        public RequestElapsedTime(IRulePersist rulePersist, TimeSpan elapsedConfig)
        {
            _rulePersist = rulePersist;
            _elapsedConfig = elapsedConfig;
        }

        public bool Execute(RequestInfo input)
        {
            var access_token_datetime_key = $"{input.Access_Token}{Constants.AppendKeyPrevReqTime}";

            var (val, fnd) = _rulePersist.Retrieve<DateTime>(access_token_datetime_key);

            if (!fnd)
            {
                _rulePersist.Put(access_token_datetime_key, DateTime.UtcNow);
                return false;
            }

            var elapsed = RulesHelper.HasElapsedTime(val, _elapsedConfig);

            if (!elapsed)
            {
                return true;
            }

            _rulePersist.Put(access_token_datetime_key, DateTime.UtcNow);

            return false;
        }
    }
}