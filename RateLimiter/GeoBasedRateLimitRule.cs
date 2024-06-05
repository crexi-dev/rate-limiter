using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
    public class GeoBasedRateLimitRule : RateLimitRule
    {
        private readonly RateLimitRule _usRule;
        private readonly RateLimitRule _euRule;
        private readonly Func<string, string> _getClientRegion;

        public GeoBasedRateLimitRule(RateLimitRule usRule, RateLimitRule euRule, Func<string, string> getClientRegion)
        {
            _usRule = usRule;
            _euRule = euRule;
            _getClientRegion = getClientRegion;
        }

        public override bool AllowRequest(string clientId)
        {
            var region = _getClientRegion(clientId);
            return region switch
            {
                "US" => _usRule.AllowRequest(clientId),
                "EU" => _euRule.AllowRequest(clientId),
                _ => true
            };
        }
    }

}
