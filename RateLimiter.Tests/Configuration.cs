using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace RateLimiter.Tests
{
    internal static class Configuration
    {
        public static IConfiguration GetConfiguration { get; } =
            new ConfigurationBuilder().AddInMemoryCollection(
                new Dictionary<string, string>
                {
                    { "maxRequestInTs", "10" },
                    { "timeRequestSpanInMs", "1000" },
                    { "timeBetweenRequestsInMs", "10" }
                }).Build();
    }
}
