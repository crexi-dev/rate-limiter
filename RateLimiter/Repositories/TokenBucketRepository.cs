using RateLimiter.Data;
using RateLimiter.Interfaces.Repositories;
using RateLimiter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Repositories
{
    public  class TokenBucketRepository : ITokenBucketRepository
    {
         
        public void AddNewTokenBucket(string token, string endpoint, TokenBucketModel tokenBucket)
        {
            //check if there are already buckets for this token
            Dictionary<string, List<TokenBucketModel>> bucketsPerEndpoint = null;
            InMemoryDatabase.TokenBucketData.TryGetValue(token, out bucketsPerEndpoint);
            if (bucketsPerEndpoint != null)//if buckets are found 
            {
                //check if there are already buckets for this endpoint
                List<TokenBucketModel> tokenBuckets = null;
                bucketsPerEndpoint.TryGetValue(endpoint, out tokenBuckets);
                if (tokenBuckets != null)
                    tokenBuckets.Add(tokenBucket);//add new bucket to existing list
                else
                {
                    List<TokenBucketModel> tokenBucketsToAdd = new List<TokenBucketModel>();
                    tokenBucketsToAdd.Add(tokenBucket);
                    bucketsPerEndpoint.Add(endpoint, tokenBucketsToAdd);
                }
            }
            else
            {
                List<TokenBucketModel> bucketsToAdd = new List<TokenBucketModel>();
                bucketsToAdd.Add(tokenBucket);

                //create new bucketlist for endpoint
                Dictionary<string, List<TokenBucketModel>> newTokenBuckets = new Dictionary<string, List<TokenBucketModel>>();
                newTokenBuckets.Add(endpoint, bucketsToAdd);

                //add new bucketlist for token
                InMemoryDatabase.TokenBucketData.Add(token, newTokenBuckets);
            }


        }

        public Dictionary<string, List<TokenBucketModel>> GetBucketsForToken(string token)
        {

            Dictionary<string, List<TokenBucketModel>> bucketsPerEndpoint;
            InMemoryDatabase.TokenBucketData.TryGetValue(token, out bucketsPerEndpoint);

            return bucketsPerEndpoint;
        }

        public List<TokenBucketModel> GetRulesForEndpoint(string token, string endpoint)
        {
            List<TokenBucketModel> rules=null;
            var bucketsForToken = GetBucketsForToken(token);
            
            if (bucketsForToken == null)
                return rules;

            bucketsForToken.TryGetValue(endpoint, out rules);

            return rules;

        }
    }
}
