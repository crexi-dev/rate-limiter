using RateLimiter.RateLimitRules;
using System;
using System.Collections.Generic;
using Xunit;
using Moq;
using Microsoft.Extensions.Caching.Memory;
using RateLimiter.Repository;

namespace RateLimiter.Tests
{
    public class RateLimiterTest
    {
        static MemoryCache _mockMemoryCache;
        public RateLimiterTest()
        {


            _mockMemoryCache = new MemoryCache(new MemoryCacheOptions());
        }
        [Fact]
        public async void Example()
        {

        //    var mockRulesProvider = new Mock<IRulesMemoryRepository>();

            //  new ResourceLimiterMiddleware()


            //object expectedValue = null;
            //mockMemoryCache.Setup(x => x.TryGetValue(It.IsAny<object>(), out expectedValue))
            //.Returns(true);

            //RulePerRequest rulePerRequest = new RulePerRequest { NumberOfRequests = 3, timeSpan = TimeSpan.FromMilliseconds(5000) };
            //RulePerTimeSpan rulePerTimeSpan = new RulePerTimeSpan { timeSpan = TimeSpan.FromMilliseconds(1) };
            //List<ILimitRule> rules = new List<ILimitRule> { rulePerRequest, rulePerTimeSpan };
            //await new RulesConfigService(_mockMemoryCache).SetRules("/", rules);


            //RulePerTimeSpan rulePerTimeSpanOntoken = new RulePerTimeSpan { timeSpan = TimeSpan.FromMilliseconds(5000) };
            //List<ILimitRule> rulesOnToken = new List<ILimitRule> { rulePerTimeSpanOntoken };
            //await new RulesConfigService(_mockMemoryCache).SetRules("tkn001", rulesOnToken);
        }


        [Fact]
        public void PassingTest()
        {

            var RulesConfigKey = "rule:/";
            List<IRateLimitRule> existingRules = null;

            _mockMemoryCache.TryGetValue(RulesConfigKey, out existingRules);
                            

            if (existingRules != null)
            {
                Assert.Equal(4, Add(2, 2));
            } 

                
        }

        [Fact]
        public void FailingTest()
        {
            Assert.Equal(5, Add(2, 2));
        }
        int Add(int x, int y)
        {
            return x + y;
        }
    }
}
