using RateLimiter.DAL;
using RateLimiter.Models;
using System.Linq;

namespace RateLimiter.Entities.Rules
{
    public class RequestCountForPeriodRuleModel: RuleModel
    {
        public int MaxRequestsCount { get; set; }
        public PeriodModel Period { get; set; }

        public RequestCountForPeriodRuleModel()
        {
            Period = new PeriodModel();
        }

        public override bool CheckRule(ClientModel client)
        {
            if (!Period.StartPeriod.HasValue || !Period.EndPeriod.HasValue)
            {
                return false;
            }

            var requestsCount = DatabaseSimulator.ClientRequests.Where(x => x.Token == client.Token && x.RequestDate >= Period.StartPeriod.Value && x.RequestDate <= Period.EndPeriod.Value).Count();
            return requestsCount < MaxRequestsCount;
        }
    }
}
