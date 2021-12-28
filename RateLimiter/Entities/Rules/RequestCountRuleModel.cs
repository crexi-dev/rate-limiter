using RateLimiter.DAL;
using RateLimiter.Models;
using System.Linq;

namespace RateLimiter.Entities.Rules
{
    public class RequestCountRuleModel: RuleModel
    {
        public int MaxRequestsCount { get; set; }

        public override bool CheckRule(ClientModel client)
        {
            var requestsCount = DatabaseSimulator.ClientRequests.Where(x => x.Token == client.Token).Count();
            return requestsCount < MaxRequestsCount;
        }
    }
}
