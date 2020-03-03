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
        /// <summary>
        /// Token info need to be maintained by other system and this method will read the token details.
        /// Data like token location, no. of requests in last min/hour/day and last request time.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public TokenInfo GetTokenInfo(string token)
        {
            return new TokenInfo() { Location = "EU", NoOfTimesCalledInLastHour = 78 };
        }
    }
}
