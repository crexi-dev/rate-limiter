using RateLimiter.Models;
using RateLimiter.Types;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Rules
{
    public interface IBaseRule
    {
        RateLimiterType RuleType { get; }
        bool Validate(RequestAttributeDataModel limitationsData, IList<RequestsHistoryModel> requestsHistoryData);
    }
}
