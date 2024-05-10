using RateLimiter.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RateLimiter.Reader
{
    public interface IRuleReader
    {
        IEnumerable<ReadRuleResponseModel> ReadRules(ReadRulesRequestModel readRulesRequestModel);
    }
}
