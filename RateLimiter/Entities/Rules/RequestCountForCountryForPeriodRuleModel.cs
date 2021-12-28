using RateLimiter.DAL;
using System.Linq;

namespace RateLimiter.Entities.Rules
{
    public class RequestCountForCountryForPeriodRuleModel: RequestCountForCountryRuleModel
    {
        public PeriodModel Period { get; set; }

        public RequestCountForCountryForPeriodRuleModel()
        {
            Period = new PeriodModel();
        }

        public new bool CheckRule(ClientModel client)
        {
            if (client.Country == null || client.Country.CountryId != CountryId || !Period.StartPeriod.HasValue || !Period.EndPeriod.HasValue)
            {
                return false;
            }

            var requestsCount = DatabaseSimulator.ClientRequests.Where(x => x.Token == client.Token && x.RequestDate >= Period.StartPeriod.Value && x.RequestDate <= Period.EndPeriod.Value).Count();
            return requestsCount < MaxRequestsCount;
        }
    }
}
