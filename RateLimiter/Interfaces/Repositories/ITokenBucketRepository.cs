using RateLimiter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Interfaces.Repositories
{
    public interface ITokenBucketRepository
    {

        Dictionary<string, List<TokenBucketModel>> GetBucketsForToken(string token);
        List<TokenBucketModel> GetRulesForEndpoint(string token, string endpoint);
        void AddNewTokenBucket(string token, string endpoint, TokenBucketModel tokenBucket);
        
    }
}


