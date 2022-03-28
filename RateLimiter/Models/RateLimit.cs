using Microsoft.EntityFrameworkCore;
using RateLimiter.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace RateLimiter.Models
{
    public class RateLimit : IRateLimit
    {
        private MyContext context;
        private DateTime currentDateTime;
        public RateLimit(MyContext myContext, DateTime dateTime)
        {
            context = myContext;
            currentDateTime = dateTime;
        }

        public void SetDateTime(DateTime dateTime)
        {
            currentDateTime = dateTime;
        }
        public void SetRules(List<RateLimitRule> rules)
        {
            rules.ForEach(q => context.Rules.Add(q));
            context.SaveChangesAsync();
        }
        public async void SetRegions(List<RateLimitRegion> regions)
        {
            regions.ForEach(q => context.Regions.Add(q));
            context.SaveChangesAsync();
        }
        public void SetResources(List<RateLimitResource> resources)
        {
            resources.ForEach(q => context.Resources.Add(q));
            context.SaveChangesAsync();
        }
        public void SetExistingRequests(List<RateLimitRequest> requests)
        {
            requests.ForEach(request =>
            {
                var item = context.Requests.Where(q => q.Id == request.Id).FirstOrDefault();

                if (item == null)
                    context.Requests.Add(request);
                else
                {
                    item.RequestDate = request.RequestDate;
                    item.RequestedTimes = request.RequestedTimes;
                    item.ResourceId = request.ResourceId;
                    item.Token = request.Token;
                }

            });
            context.SaveChangesAsync();
        }
        public bool ValidateToken(RateLimitToken token)
        {
            var rules = context.Rules.Where(q => q.ResourceId == token.ResourceId && q.RegionId == token.RegionId).ToList();
            if (rules.Count == 0)//no rules applied for this resource and this region
                return true;

            var request = context.Requests.Where(q => q.Token == token.Token && q.ResourceId == token.ResourceId).FirstOrDefault();
            if (request == null)//it is the first request for this resource and this client/token
                return true;

            bool isValid = false;
            foreach (var rule in rules)
            {
                switch (rule.RuleType)
                {
                    case RateLimitRuleType.RequestsPerTimespan:
                        isValid = (currentDateTime.Subtract(request.RequestDate) >= rule.TimeSpanAllowed || rule.NumberRequestsAllowed > request.RequestedTimes);

                        break;
                    case RateLimitRuleType.TimespanPassedSinceLastCall:
                        isValid = (currentDateTime.Subtract(request.RequestDate) >= rule.TimeSpanAllowed);

                        break;
                    default://another unknown rule, break;
                        isValid = false;
                        break;
                }

                if (!isValid)
                    break;
            }

            return isValid;
        }
    }
}
