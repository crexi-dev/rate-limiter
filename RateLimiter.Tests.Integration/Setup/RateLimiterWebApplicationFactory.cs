using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace RateLimiter.Tests.Integration.Setup
{
    public class RateLimiterWebApplicationFactory : WebApplicationFactory<Program>
    {
        public TestServer CreateServer(IWebHostBuilder builder)
        {
            //builder.ConfigureServices(services =>
            //{
            //    services.AddSingleton<IStartupFilter, IpStartupFilter>();
            //});

            return base.CreateServer(builder);
        }
    }
}
