using System;
using System.Linq;

namespace RateLimiter.DataStore
{
    public interface IRuleAStore
    {
        void InsertTokenInformation(string token, DateTime lastRequestDateTime, int remainingRequests);
        RuleAStore? GetRuleAByToken(string token);
    }

    public class RuleAStore : IRuleAStore
    {
        public string Token { get; set; } = null!;
        public DateTime LastRequestDateTime { get; set; }
        public int RemainingRequests { get; set; }

        public void InsertTokenInformation(string token, DateTime lastRequestDateTime, int remainingRequests)
        {
            DataStore.RuleAStores.Add(new RuleAStore()
            {
                Token = token,
                LastRequestDateTime = lastRequestDateTime,
                RemainingRequests = remainingRequests
            });
        }

        public RuleAStore? GetRuleAByToken(string token)
        {
            return DataStore.RuleAStores.SingleOrDefault(x => x.Token == token);
        }
    }
}