using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.DataStore
{
    public interface IRuleBStore
    {
        void InsertTokenInformation(string token, DateTime lastRequestDateTime);
        RuleBStore? GetByToken(string token);
        void UpdateRuleBTokenInfo(string token, DateTime lastRequestDateTime);
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

        public RuleBStore? GetByToken(string token)
        {
            return DataStore.RuleBStores.SingleOrDefault(x => x.Token == token);
        }

        public void UpdateRuleBTokenInfo(string token, DateTime lastRequestDateTime)
        {
            var tokenInfo = GetByToken(token);
            if (tokenInfo == null) throw new KeyNotFoundException("Token does not exist in the datastore");
            tokenInfo.LastRequestDateTime = lastRequestDateTime;
        }
    }
}