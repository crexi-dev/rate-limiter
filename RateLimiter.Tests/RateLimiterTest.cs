using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTest
    {
        private readonly LimiterService _limiterService;
        private readonly IEndpointService _endpointService;

        public RateLimiterTest()
        {
            _endpointService = new FakeEndpointService();
            _limiterService = new LimiterService(_endpointService);
            _limiterService.Configure(new LimiterConfiguration
            {
                LimitRules = new List<LimitRule>()
                {
                    new LimitRule{ 
                        EndPoints = new List<string>(){ "USBasedEndpoint1", "USBasedEndpoint2"},
                        RuleType = RuleType.RequestsPerPeriod,
                        Period = Period.Second,
                        Value = 2
                    },
                    new LimitRule
                    {
                        EndPoints = new List<string>() { "EUBasedEndpoint1" },
                        RuleType = RuleType.LastCallPassed,
                        Period = Period.Second,
                        Value = 3
                    }
                }
            });
        }

        [Test]
        public async Task ThirdRequestPerPeriodFail()
        {
            var allowed = await _limiterService.ProccessRequest("TokenTest1").ConfigureAwait(false);
            Assert.IsTrue(allowed);
            var allowed2 = await _limiterService.ProccessRequest("TokenTest1").ConfigureAwait(false);
            Assert.IsTrue(allowed2);
            var allowed3 = await _limiterService.ProccessRequest("TokenTest1").ConfigureAwait(false);
            Assert.IsFalse(allowed3);
        }

        [Test]
        public async Task LastCallNotPassedFail()
        {
            var allowed = await _limiterService.ProccessRequest("TokenTest3").ConfigureAwait(false);
            Assert.IsTrue(allowed);
            var allowed2 = await _limiterService.ProccessRequest("TokenTest3").ConfigureAwait(false);
            Assert.IsFalse(allowed2);
            await Task.Delay(3000);
            var allowed3 = await _limiterService.ProccessRequest("TokenTest3").ConfigureAwait(false);
            Assert.IsTrue(allowed3);
        }
    }
}
