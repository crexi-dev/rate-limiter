using FluentAssertions.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Text;
using System.Text.Json;
using Xunit;

namespace RateLimit.Tests.Integration
{
	
	public class RateLimiterMiddlewareTest
	{
		[Fact]
		public async Task MiddlewareTest_ReturnsNotFoundForRequest()
		{
			var config = new ConfigurationBuilder().AddJsonFile("TestConfig.json", optional: true, reloadOnChange: true).Build();
			using var host = await new HostBuilder()
				.ConfigureWebHost(webBuilder =>
				{
					webBuilder
						.UseTestServer()
						.ConfigureServices(services =>
						{
							services.AddRateRuleAService(config);
						})
						.Configure(app =>
						{
							app.UseRateLimiterMiddleware();							
						});
				})
				.StartAsync();

			var server = host.GetTestServer();
			server.BaseAddress = new Uri("https://example.com/");

			var context = await server.SendAsync(c =>
			{
				c.Request.Method = HttpMethods.Post;
				c.Request.Headers["access_token"] = new string[2] { "Client1", "EU" };
				c.Request.QueryString = new QueryString("?and=query");
			});

			Assert.Equal("example.com", context.Request.Host.Value);
		}		
	}
}
