namespace RateLimiter.Services
{
    public class RateLimitingService
    {
        private readonly RuleProvider _ruleProvider;

        public RateLimitingService(RuleProvider ruleProvider)
        {
            _ruleProvider = ruleProvider;
        }

        public bool IsRequestAllowed(string resource, string token)
        {
            string region = token.Split('-')[0];
            var rules = _ruleProvider.GetRulesForResource(resource, region);
            foreach (var rule in rules)
            {
                if (!rule.IsAllowed(token))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
