using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using RateLimiter.Models.Options;

namespace RateLimiter.Tests.Integration.Setup
{
    public class BaseClassFixture : IClassFixture<RateLimiterWebApplicationFactory>
    {
        public HttpClient HttpClient;

        public ActiveProcessorsOptions ActiveProcessorsOptions { get; set; }
        public LastCallTimeSpanOptions LastCallTimeSpanOptions { get; set; }
        public RequestRateOptions RequestRateOptions { get; set; }

        protected BaseClassFixture(RateLimiterWebApplicationFactory factory)
        {
            HttpClient = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((context, conf) =>
                {
                    conf.AddJsonFile("appsettings.test.json");
                });
            }).CreateClient(options: new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("https://localhost:44362"),
            });

            var config = InitConfiguration();
            ActiveProcessorsOptions = config.Get<ActiveProcessorsOptions>();
            LastCallTimeSpanOptions = config.Get<LastCallTimeSpanOptions>();
            RequestRateOptions = config.Get<RequestRateOptions>();
        }

        private static IConfiguration InitConfiguration()
        {
            var config = new ConfigurationBuilder()
               .AddJsonFile("appsettings.test.json")
                .AddEnvironmentVariables()
                .Build();
            return config;
        }
    }
}
