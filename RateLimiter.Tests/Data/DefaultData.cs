using Microsoft.EntityFrameworkCore;
using RateLimiter.DAL;
using RateLimiter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Tests.Data
{
    public static class DefaultData
    {
        public static MyContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<MyContext>()
                .UseInMemoryDatabase(databaseName: "MyDatabase")
                .Options;

            return new MyContext(options);
        }
        public static List<RateLimitRegion> GetRegions()
        {
            var list = new List<RateLimitRegion>();

            list.Add(new RateLimitRegion()
            {
                Id = 1,
                RegionBase = "US",
                RegionName = "United States"
            });

            list.Add(new RateLimitRegion()
            {
                Id = 2,
                RegionBase = "EU",
                RegionName = "Europe"
            });

            return list;
        }

        public static List<RateLimitClient> GetClients()
        {
            var list = new List<RateLimitClient>();

            list.Add(new RateLimitClient()
            {
                Id = 1,
                ClientName = "Bethoven"
            });

            return list;
        }

        public static List<RateLimitResource> GetResources()
        {
            var list = new List<RateLimitResource>();

            list.Add(new RateLimitResource()
            {
                Id = 1,
                EndpointName = "CreateUser",
                Identifier = "CreateUser(User user)"
            });

            list.Add(new RateLimitResource()
            {
                Id = 2,
                EndpointName = "SearchUser",
                Identifier = "SearchUser(string filter)"
            });

            return list;
        }

        public static List<RateLimitRule> GetRules()
        {
            var list = new List<RateLimitRule>();

            //US rules

            list.Add(new RateLimitRule()//US allows 5 RequestsPerTimespan in a timespan of 5 secs for Resource "CreateUser"
            {
                ResourceId = GetResources()[0].Id,
                RegionId = GetRegions()[0].Id,
                NumberRequestsAllowed = 5,
                TimeSpanAllowed = new TimeSpan(0, 0, 5),
                RuleType = RateLimitRuleType.RequestsPerTimespan
            });

            list.Add(new RateLimitRule()//US has 1 TimespanPassedSinceLastCall for Resource "CreateUser"
            {
                ResourceId = GetResources()[0].Id,
                RegionId = GetRegions()[0].Id,
                TimeSpanAllowed = new TimeSpan(0, 0, 1),
                RuleType = RateLimitRuleType.TimespanPassedSinceLastCall
            });

            //EU rules
            list.Add(new RateLimitRule()//EU allows 50 RequestsPerTimespan in a timespan of 10 secs for Resource "CreateUser"
            {
                ResourceId = GetResources()[0].Id,
                RegionId = GetRegions()[1].Id,
                NumberRequestsAllowed = 50,
                TimeSpanAllowed = new TimeSpan(0, 0, 10),
                RuleType = RateLimitRuleType.RequestsPerTimespan
            });

            list.Add(new RateLimitRule()//EU has 5 TimespanPassedSinceLastCall for Resource "CreateUser"
            {
                ResourceId = GetResources()[0].Id,
                RegionId = GetRegions()[1].Id,
                TimeSpanAllowed = new TimeSpan(0, 0, 5),
                RuleType = RateLimitRuleType.TimespanPassedSinceLastCall
            });

            //...
            //...
            //...
            //...
            //Add more rules here as needed

            return list;
        }
        public static List<RateLimitRequest> GetExistingRequests(string token, int resourceId, DateTime requestDate, int requestedTimes)
        {
            var list = new List<RateLimitRequest>();

            list.Add(new RateLimitRequest()
            {
                Id = 1,
                Token = "ABC-0001",
                ResourceId = GetResources()[0].Id,
                RequestDate = requestDate,
                RequestedTimes = requestedTimes
            });

            return list;
        }        
    }
}
