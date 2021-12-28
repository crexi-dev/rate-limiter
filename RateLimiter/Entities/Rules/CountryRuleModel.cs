using RateLimiter.Models;

namespace RateLimiter.Entities.Rules
{
    public class CountryRuleModel: RuleModel
    {
        public int? CountrId { get; set; }

        public override bool CheckRule(ClientModel client)
        {
            return client != null && client.Country != null && CountrId.HasValue && client.Country.CountryId== CountrId.Value;
        }
    }
}
