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
    public class TokenIntervalRepository : ITokenIntervalRepository
    {
        public void AddNewTokenInterval(string token, string endpoint, TokenIntervalModel tokenInterval)
        {
            //check if there are already intervals  for this token
            Dictionary<string, List<TokenIntervalModel>> intervalsPerEndpoint = null;
            InMemoryDatabase.TokenIntervalData.TryGetValue(token, out intervalsPerEndpoint);
            if (intervalsPerEndpoint != null)//if intervals are found 
            {
                //check if there are already intervals for this endpoint
                List<TokenIntervalModel> tokenIntervals = null;
                intervalsPerEndpoint.TryGetValue(endpoint, out tokenIntervals);
                if (tokenIntervals != null)
                    tokenIntervals.Add(tokenInterval);//add new interval to existing list
                else
                {
                    List<TokenIntervalModel> tokenIntervalsToAdd = new List<TokenIntervalModel>();
                    tokenIntervalsToAdd.Add(tokenInterval);
                    intervalsPerEndpoint.Add(endpoint, tokenIntervalsToAdd);
                }
            }
            else
            {
                List<TokenIntervalModel> intervalsToAdd = new List<TokenIntervalModel>();
                intervalsToAdd.Add(tokenInterval);

                //create new interval list  for endpoint
                Dictionary<string, List<TokenIntervalModel>> newTokenIntervals = new Dictionary<string, List<TokenIntervalModel>>();
                newTokenIntervals.Add(endpoint, intervalsToAdd);

                //add new interval list for token
                InMemoryDatabase.TokenIntervalData.Add(token, newTokenIntervals);
            }
        }

        public List<TokenIntervalModel> GetRulesForEndpoint(string token, string endpoint)
        {
            List<TokenIntervalModel> rules = null;
            var intervalsForToken = GetRulesForToken(token);

            if (intervalsForToken == null)
                return rules;

            intervalsForToken.TryGetValue(endpoint, out rules);

            return rules;
        }

        public Dictionary<string, List<TokenIntervalModel>> GetRulesForToken(string token)
        {
            Dictionary<string, List<TokenIntervalModel>> intervalsPerEndpoint;
            InMemoryDatabase.TokenIntervalData.TryGetValue(token, out intervalsPerEndpoint);

            return intervalsPerEndpoint;
        }
    }
}
