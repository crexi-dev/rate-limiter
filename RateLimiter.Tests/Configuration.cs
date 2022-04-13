using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace RateLimiter.Tests
{
    internal static class Configuration
    {
        // For testing, create a default IConfiguration object with some hard-coded configuration options
        public static IConfiguration Default { get; } =
            new ConfigurationBuilder()
                     .AddInMemoryCollection(new Dictionary<string, string>
                        {
                            {"RequestsPerTimespanRule_MaxRequests", "10"},
                            {"TimeSinceLastRequestRule_Ms", "10" },
                        })
                     .Build();
    }
}
