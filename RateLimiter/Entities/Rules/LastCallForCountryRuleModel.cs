using System;

namespace RateLimiter.Entities.Rules
{
    public class LastCallForCountryRuleModel: LastCallRuleModel
    {
        public int? CountryId { get; set; }

        public new bool CheckRule(ClientModel client)
        {
            var countryCheck = (client.Country != null && CountryId.HasValue) ? client.Country.CountryId == CountryId.Value : true;
            return base.CheckRule(client) && countryCheck;
        }
    }
}
