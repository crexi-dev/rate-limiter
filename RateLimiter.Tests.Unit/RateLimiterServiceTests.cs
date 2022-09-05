using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Moq;
using RateLimiter.Models;
using RateLimiter.Models.Options;
using RateLimiter.RateLimiterProcessors;
using RateLimiter.Services;
using RateLimiter.Stores;
using System;

namespace RateLimiter.Tests.Unit
{
    public class RateLimiterServiceTests
    {
        private class Setup
        {
            public RateLimiterService RateLimiterService { get; set; }

            public Mock<ICacheProvider> Cache { get; set; }
            public IEnumerable<IRateLimiterProcessor> Processors { get; set; }// = new Mock<IEnumerable<IRateLimiterProcessor>>();
            public IOptions<ActiveProcessorsOptions> Options { get; set; }

            public Setup()
            {
                this.Cache = new Mock<ICacheProvider>();
                Cache.Setup(cp => cp.Get<string>(It.IsAny<string>())).Returns((string arg) => TestCache[arg]);
                Cache.Setup(cp => cp.Set<It.IsAnyType>(It.IsAny<string>(), It.IsAny<It.IsAnyType>())).Callback((string clientId, List<DateTime> newRequestTimes) =>
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

                var activeProcessorsOptions = new ActiveProcessorsOptions
                {
                    ActiveProcessorNames = new string[] { ProcessorName.LastCallTimeSpan.ToString(), ProcessorName.RequestRate.ToString() }
                };
                this.Options = Microsoft.Extensions.Options.Options.Create(activeProcessorsOptions);

                Mock<IRateLimiterProcessor> mockLcts = new Mock<IRateLimiterProcessor>();
                mockLcts.Setup(prc => prc.Process(It.IsAny<IList<DateTime>>())).Returns(new RateLimiterProcessorResponse(ProcessorName.LastCallTimeSpan.ToString()));

                Mock<IRateLimiterProcessor> mockRr = new Mock<IRateLimiterProcessor>();
                mockRr.Setup(prc => prc.Process(It.IsAny<IList<DateTime>>())).Returns(new RateLimiterProcessorResponse(ProcessorName.RequestRate.ToString()));

                IEnumerable<IRateLimiterProcessor> mockProcessors = new List<IRateLimiterProcessor>()
                {
                    mockLcts.Object,
                    mockRr.Object
                };

                RateLimiterService = new RateLimiterService(Cache.Object, mockProcessors, this.Options);
            }

            public Dictionary<string, List<DateTime>> TestCache = new Dictionary<string, List<DateTime>>();
        }

        [Fact]
        public void Example()
        {
            // Arrange
            var tSetup = new Setup();

            // Act
            // Assert
        }
    }
}
