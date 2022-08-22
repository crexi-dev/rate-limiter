using System;
using System.Linq;

namespace RateLimiter.DataStore
{
    public interface IRuleBStore
    {
        void InsertTokenInformation(string token, DateTime lastRequestDateTime);
        RuleBStore? GetRuleAByToken(string token);
    }

    public class RuleBStore : IRuleBStore
    {
        public string Token { get; set; } = null!;
        public DateTime LastRequestDateTime { get; set; }
        
        public void InsertTokenInformation(string token, DateTime lastRequestDateTime)
        {
            DataStore.RuleBStores.Add(new RuleBStore()
            {
                Token = token,
                LastRequestDateTime = lastRequestDateTime
            });
        }

        public RuleBStore? GetRuleAByToken(string token)
        {
            return DataStore.RuleBStores.SingleOrDefault(x => x.Token == token);
        }
    }
}