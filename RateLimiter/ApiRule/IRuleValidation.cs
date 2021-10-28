using RateLimiter.Model.Enum;
using RateLimiter.Model.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter.ApiRule
{
    public interface IRuleValidation
    {
        bool Validate(string token, ResourceEnum resourceName, List<ApiRequest> tokenRequestLog);
    }
}
