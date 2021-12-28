using RateLimiter.DAL;
using System.Linq;

namespace RateLimiter.Entities.Rules
{
    public class RequestCountForCountryRuleModel: RequestCountRuleModel
    {
        public int CountryId { get; set; }

        public new bool CheckRule(ClientModel client)
        {
            if (client.Country == null || client.Country.CountryId != CountryId)
            {
                return false;
            }

            var requestsCount = DatabaseSimulator.ClientRequests.Where(x => x.Token == client.Token).Count();
            return requestsCount <= MaxRequestsCount;
        }
    }
}
