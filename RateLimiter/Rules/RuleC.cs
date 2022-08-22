namespace RateLimiter.Rules
{
    public class RuleC : IRule
    {
        private int UsBasedPeriod { get; set; }
        
        private int UsBasedAllowedNumberOfRequests { get; set; }
        
        private int EuBasedPeriod { get; set; }
        
        private RuleC(int usBasedPeriod, int usBasedAllowedNumberOfRequests, int euBasedPeriod)
        {
            UsBasedPeriod = usBasedPeriod;
            UsBasedAllowedNumberOfRequests = usBasedAllowedNumberOfRequests;
            EuBasedPeriod = euBasedPeriod;
        }

        public static RuleC Configure(int usBasedPeriod, int usBasedAllowedNumberOfRequests, int euBasedPeriod)
        {
            return new(usBasedPeriod, usBasedAllowedNumberOfRequests, euBasedPeriod);
        }
        
        public bool Execute(string token)
        {
            if (token.StartsWith("US"))
            {
                var rule = RuleA.Configure(UsBasedPeriod, UsBasedAllowedNumberOfRequests);
                return rule.Execute(token);
            }

            if (token.StartsWith("EU"))
            {
                var rule = RuleB.Configure(EuBasedPeriod);
                return rule.Execute(token);
            }

            return false;
        }
    }
}