using RateLimiter.RateLimitRules;
using System;
using System.Collections.Generic;
using Xunit;
using Microsoft.Extensions.Caching.Memory;
using RateLimiter.Repository;


namespace RateLimiter.Tests
{
    public class when_using_request_rate_limiter
    {
        static MemoryCache _memoryCache;
        static RulesRepository _rulesRepo;
        static EventsRepository _eventsRepo;
        static RulesConfigService _rulesConfigService;
        static RulesProcessorService _rulesProcessorService;

        public when_using_request_rate_limiter()
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions());

            _rulesRepo = new RulesRepository(_memoryCache);
            _eventsRepo = new EventsRepository(_memoryCache);

            _rulesConfigService = new RulesConfigService(_rulesRepo);
            _rulesProcessorService = new RulesProcessorService(_eventsRepo, _rulesRepo);


        }

        [Theory]
        [InlineData(1000, 1, 0, true)]
        [InlineData(1000, 5, 1000, false)]
        [InlineData(1000, 10, 1100, true)]
        public void then_simple_rule_should_work_on_given_endpoint(int rulePerTimespanWindow, int requestCount, int requestInterval, bool expected)
        {

            string ruleKey = "/myuri"; // fake endpoint


            // configure rule to limit requests on endpoint(/myuri) within ruleWindow(milliseconds)
            RulePerTimeSpan rulePerTimeSpan = new() { TimeSpanInMilliseconds = rulePerTimespanWindow };
            List<IRateLimitRule> rules = new() { rulePerTimeSpan };
            _rulesConfigService.SetRules(ruleKey, rules);


            RequestEventGenerator(ruleKey, requestCount, requestInterval);

            var result = _rulesProcessorService.IsValidLimit(new List<string> { ruleKey });

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(3, 1000, 1, 0, true)]
        [InlineData(50, 1000, 105, 1000, false)]
        [InlineData(10, 1000, 10, 1100, true)]
        [InlineData(2, 100, 15, 100, false)]
        [InlineData(35, 5000, 35, 4000, false)]
        public void then_set_of_different_rules_should_work_on_multiple_keys(int numberOfRequestsToCheck, int rulePerTimespanWindow, int requestCount, int requestInterval, bool expected)
        {


            string ruleKey_endpoint = "/differentURI"; // fake endpoint
            string ruleKey_Token = "GciOiJIUzI1NiIsI"; // fake token


            // configure rule to limit requests on endpoint(/myuri) within rule window(milliseconds)
            RulePerTimeSpan rulePerTimeSpan = new() { TimeSpanInMilliseconds = rulePerTimespanWindow };
            _rulesConfigService.SetRules(ruleKey_endpoint, new() { rulePerTimeSpan });


            // configure different rules on header parameter(token) withing rule window and limit requests per timespan
            RulePerRequest rulePerRequestOnToken = new() { NumberOfRequests = numberOfRequestsToCheck, TimeSpanInMilliseconds = rulePerTimespanWindow };
            RulePerTimeSpan rulePerTimeSpanOntoken = new RulePerTimeSpan { TimeSpanInMilliseconds = rulePerTimespanWindow };
            _rulesConfigService.SetRules(ruleKey_endpoint, new() { rulePerRequestOnToken, rulePerTimeSpanOntoken });

            RequestEventGenerator(ruleKey_endpoint, requestCount, requestInterval);
            RequestEventGenerator(ruleKey_Token, requestCount, requestInterval);

            var result = _rulesProcessorService.IsValidLimit(new List<string> { ruleKey_endpoint, ruleKey_Token });

            Assert.Equal(expected, result);

        }


        private void RequestEventGenerator(string ruleKey, int requestCount, double requestInterval)
        {

            var date = DateTimeOffset.UtcNow;
            for (int i = 0; i < requestCount; i++)
            {
                _rulesProcessorService.AddRequestEvent(ruleKey, new() { RequestDate = date });
                date = date.AddMilliseconds(requestInterval);
            }
        }


    }
}
