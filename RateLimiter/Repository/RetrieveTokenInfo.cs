using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RateLimiter.Model;

namespace RateLimiter.Repository
{
    public interface IRetrieveTokenInfo
    {
        TokenInfo GetTokenInfo(string token);
    }

    public class RetrieveTokenInfo : IRetrieveTokenInfo
    {
        public TokenInfo GetTokenInfo(string token)
        {
            return new TokenInfo() { Location = "EU", NoOfTimesCalledInLastHour = 78 };
        }
    }
}
