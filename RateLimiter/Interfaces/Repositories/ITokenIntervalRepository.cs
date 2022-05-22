using RateLimiter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Interfaces.Repositories
{
    public interface ITokenIntervalRepository
    {
        Dictionary<string, List<TokenIntervalModel>> GetRulesForToken(string token);
        List<TokenIntervalModel> GetRulesForEndpoint(string token, string endpoint);
        void AddNewTokenInterval(string token, string endpoint, TokenIntervalModel tokenBucket);
    }
}
