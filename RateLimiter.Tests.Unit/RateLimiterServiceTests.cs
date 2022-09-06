using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json.Linq;
using RateLimiter.Models;
using RateLimiter.Models.Options;
using RateLimiter.RateLimiterProcessors;
using RateLimiter.Services;
using RateLimiter.Stores;
using System.Collections.Generic;

namespace RateLimiter.Tests.Unit
{
    public class RateLimiterServiceTests
    {
        private class Setup
        {
            public RateLimiterService RateLimiterService { get; set; }

            public Mock<ICacheProvider> Cache { get; set; }
            public IEnumerable<IRateLimiterProcessor> Processors { get; set; }
            public IOptions<ActiveProcessorsOptions> Options { get; set; }

            public Setup()
            {
                this.Cache = new Mock<ICacheProvider>();
                CreateMockCache_Get();

                CreateMockCache_Set();

                IEnumerable<IRateLimiterProcessor> mockProcessors = CreateMockProcessors();

                RateLimiterService = new RateLimiterService(Cache.Object, mockProcessors, this.Options);
            }

            // TODO: TestCache is being populated on Setup as functions run then and again during a test. TestCache should be local to each test.
            public Dictionary<string, List<DateTime>> TestCache = new Dictionary<string, List<DateTime>>();

            private void CreateMockCache_Set()
            {
                Cache.Setup(cp => cp.Set<List<DateTime>>(It.IsAny<string>(), It.IsAny<List<DateTime>>())).Callback((string clientId, List<DateTime> newRequestTimes) =>
                {
                    bool clientExists = TestCache.TryGetValue(clientId, out List<DateTime> oldRequestTimes);
                    if (!clientExists)
                    {
                        TestCache.Add(clientId, newRequestTimes);
                    }

                    if (clientExists)
                    {
                        TestCache[clientId] = newRequestTimes;
                    }
                });
            }

            private void CreateMockCache_Get()
            {
                Cache.Setup(cp => cp.Get<List<DateTime>>(It.IsAny<string>())).Returns((string clientId) =>
                {
                    bool keyExists = TestCache.ContainsKey(clientId);
                    List<DateTime> requestTimes = new();
                    if (!keyExists)
                    {
                        requestTimes = new List<DateTime> { DateTime.UtcNow };
                    }

                    if (keyExists)
                    {
                        requestTimes = TestCache[clientId];
                        requestTimes.Add(DateTime.UtcNow);
                    }

                    TestCache.Add(clientId, requestTimes);

                    return requestTimes;
                });
            }

            private IEnumerable<IRateLimiterProcessor> CreateMockProcessors()
            {
                var activeProcessorsOptions = new ActiveProcessorsOptions
                {
                    ActiveProcessorNames = new string[] { ProcessorName.LastCallTimeSpan.ToString(), ProcessorName.RequestRate.ToString() }
                };
                this.Options = Microsoft.Extensions.Options.Options.Create(activeProcessorsOptions);

                Mock<IRateLimiterProcessor> mockLcts = new();
                mockLcts.SetupGet(prc => prc.Name).Returns(ProcessorName.LastCallTimeSpan);
                mockLcts.Setup(prc => prc.Process(It.IsAny<IList<DateTime>>())).Returns(new RateLimiterProcessorResponse(ProcessorName.LastCallTimeSpan.ToString()));

                Mock<IRateLimiterProcessor> mockRr = new();
                mockRr.SetupGet(prc => prc.Name).Returns(ProcessorName.RequestRate);
                mockRr.Setup(prc => prc.Process(It.IsAny<IList<DateTime>>())).Returns(new RateLimiterProcessorResponse(ProcessorName.RequestRate.ToString()));

                IEnumerable<IRateLimiterProcessor> mockProcessors = new List<IRateLimiterProcessor>()
                {
                    mockLcts.Object,
                    mockRr.Object
                };
                return mockProcessors;
            }
        }

        [Fact]
        public void ProcessRequest_Success()
        {
            // Arrange
            var tSetup = new Setup();
            var clientId = "::1";
            var timestamp = DateTime.UtcNow;

            // Act
            var responses = tSetup.RateLimiterService.ProcessRequest(clientId, timestamp);

            // Assert
            Assert.NotNull(responses);
            Assert.NotEmpty(responses);
            
            var firstResponse = responses[0];
            Assert.True(firstResponse.RateLimterProcessor.Equals(ProcessorName.LastCallTimeSpan.ToString()));
        }
    }
}
