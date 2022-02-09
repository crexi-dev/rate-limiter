using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using RateLimiter.Enums;
using RateLimiter.Models;

namespace RateLimiter.Tests
{
    public static class MockDataGenerator
    {
        private static readonly List<RuleModel> Rules;
        
        static MockDataGenerator()
        {
            Rules = new List<RuleModel>();
            
            ReadJson();
        }
        
        public static IEnumerable<RequestModel> GetCorrectUsData()
        {
            var mockRequests = new List<RequestModel>();
            var rule = Rules.First(r => r.Location == LocationEnum.US);
            
            for (var i = 0; i < rule.AllowedRequests; ++i)
            {
                mockRequests.Add(new RequestModel
                {
                    Token = $"{rule.Location}{i}",
                    RequestFired = DateTime.Now
                });
            }

            return mockRequests;
        }

        public static List<RequestModel> GetIncorrectUsData()
        {
            var mockRequests = new List<RequestModel>();
            var rule = Rules.First(r => r.Location == LocationEnum.US);

            for (var i = 0; i < rule.AllowedRequests + 1; ++i)
            {
                mockRequests.Add(new RequestModel
                {
                    Token = $"{rule.Location}{i}",
                    RequestFired = DateTime.Now
                });
            }

            return mockRequests;
        }

        public static IEnumerable<RequestModel> GetCorrectEuData()
        {
            var mockRequests = new List<RequestModel>();
            var rule = Rules.First(r => r.Location == LocationEnum.EU);
            var dateTime = DateTime.Now.Subtract(TimeSpan.FromMinutes(1));
            
            for (var i = 0; i < rule.AllowedRequests; ++i)
            {
                mockRequests.Add(new RequestModel
                {
                    Token = $"{rule.Location}{i}",
                    RequestFired = dateTime.AddSeconds(rule.Interval.Seconds)
                });
            }

            return mockRequests;
        }
        
        public static List<RequestModel> GetIncorrectEuData()
        {
            var mockRequests = new List<RequestModel>();
            var rule = Rules.First(r => r.Location == LocationEnum.EU);
            
            for (var i = 0; i < rule.AllowedRequests; ++i)
            {
                mockRequests.Add(new RequestModel
                {
                    Token = $"{rule.Location}{i}",
                    RequestFired = DateTime.Now
                });
            }

            return mockRequests;
        }

        public static IEnumerable<RequestModel> GetCorrectBothRuleData()
        {
            var mockRequests = new List<RequestModel>();
            var dateTime = DateTime.Now.Subtract(TimeSpan.FromMinutes(1));
            var rule = Rules.First(r => r.Location == LocationEnum.US);

            
            for (var i = 0; i < rule.AllowedRequests; ++i)
            {
                mockRequests.Add(new RequestModel
                {
                    Token = $"{rule.Location}{i}",
                    RequestFired = dateTime.AddSeconds(rule.Interval.Seconds)
                });
            }

            return mockRequests;
        }

        public static List<RequestModel> GetIncorrectBothRuleData()
        {
            var mockRequests = new List<RequestModel>();
            var rule = Rules.First(r => r.Location == LocationEnum.US);

            
            for (var i = 0; i < rule.AllowedRequests + 1; ++i)
            {
                mockRequests.Add(new RequestModel
                {
                    Token = $"{rule.Location}{i}",
                    RequestFired = DateTime.Now
                });
            }

            return mockRequests;
        }

        #region Helper methods

        private static void ReadJson()
        {
            using var streamReader = new StreamReader("config.json");
            var json = streamReader.ReadToEnd();
            var data = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, RuleModel>>>(json);
            if (data == null) return;
            
            foreach (var (key, value) in data["Rules"])
            {
                value.Location = (LocationEnum)Enum.Parse(typeof(LocationEnum), key);
                Rules.Add(value);
            }
        }

        #endregion
    }
}