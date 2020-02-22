using RateLimiter.State;
using System;

namespace RateLimiter.Rules
{
    public class RequestPerElapsedTime : IRule<RequestInfo>
    {
        private readonly IRulePersist _rulePersist;
        private readonly TimeSpan _elapsedConfig;
        private readonly int _reqLimtConfig;

        public RequestPerElapsedTime(IRulePersist rulePersist, TimeSpan elapsedConfig, int reqLimtConfig)
        {
            _rulePersist = rulePersist;
            _elapsedConfig = elapsedConfig;
            _reqLimtConfig = reqLimtConfig;
        }

        public bool Execute(RequestInfo input)
        {
            var access_token_datetime_key = $"{input.Access_Token}{Constants.AppendKeyPrevReqTime}";
            var access_token_req_cnt_key = $"{input.Access_Token}{Constants.AppendKeyReqCnt}";

            var prevDateOffset = _rulePersist.Retrieve<DateTime>(access_token_datetime_key);
            var cnt = _rulePersist.Retrieve<int>(access_token_req_cnt_key);

            if (!prevDateOffset.fnd)
            {
                _rulePersist.Put(access_token_datetime_key, DateTime.UtcNow);
                _rulePersist.Put(access_token_req_cnt_key, 1);

                return false;
            }

            var elapsed = RulesHelper.HasElapsedTime(prevDateOffset.val, _elapsedConfig);

            var currCnt = cnt.val + 1;

            _rulePersist.Put(access_token_req_cnt_key, currCnt);

            if (!elapsed && currCnt > _reqLimtConfig)
            {
                return true;
            }

            if (elapsed)
            {
                _rulePersist.Put(access_token_datetime_key, DateTime.UtcNow);
                _rulePersist.Put(access_token_req_cnt_key, 1);
                return false;
            }

            return false;
        }
    }
}