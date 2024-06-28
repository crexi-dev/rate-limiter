using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using RateLimiter.Middleware;
using RateLimiter.RateLimiter.Models;
using System;
using System.Threading.Tasks;

namespace RateLimiter.Tests
{
    public class RateLimitingMiddlewareTest
    {
        [Test]
        public async Task FixedWindowRateLimiter_TwoRequests_ShouldLimitSecondRequest()
        {
            using var host = await new HostBuilder()
                .ConfigureWebHost(webBuilder =>
                {
                    webBuilder
                        .UseTestServer()
                        .ConfigureServices(services =>
                        {
                            services.AddRouting();
                            services.AddFixedWindowRateLimiter(
                                policy: "policy1",
                                region: Region.EU,
                                configureOptions: x =>
                                {
                                    x.Window = TimeSpan.FromSeconds(3);
                                    x.Limit = 1;
                                });

                            services.AddRateLimiter();
                        })
                        .Configure(app =>
                        {
                            app.UseRouting();
                            app.UseMiddleware<RateLimitingMiddleware>();
                            app.UseEndpoints(endpoints =>
                            {
                                endpoints
                                    .MapGet("/test", () => "Test responce")
                                    .RequireRateLimiting("policy1", Region.EU);
                            });

                            app.UseRateLimiter();
                        });
                })
                .StartAsync();

            var client = host.GetTestClient();

            client.DefaultRequestHeaders.Add("Authorization", "test-token");
            client.DefaultRequestHeaders.Add("Region", "EU");

            var response1 = await client.GetAsync("/test");
            var response2 = await client.GetAsync("/test");

            Assert.True(response1.IsSuccessStatusCode);
            Assert.True(response2.StatusCode == System.Net.HttpStatusCode.TooManyRequests);
        }

        [Test]
        public async Task TimeSpanRateLimiter_TwoRequests_ShouldLimitSecondRequest()
        {
            using var host = await new HostBuilder()
                .ConfigureWebHost(webBuilder =>
                {
                    webBuilder
                        .UseTestServer()
                        .ConfigureServices(services =>
                        {
                            services.AddRouting();

                            services.AddTimespanRateLimiter(
                                policy: "policy2",
                                region: Region.US,
                                configureOptions: x =>
                                {
                                    x.Window = TimeSpan.FromSeconds(3);
                                });

                            services.AddRateLimiter();
                        })
                        .Configure(app =>
                        {
                            app.UseRouting();
                            app.UseMiddleware<RateLimitingMiddleware>();
                            app.UseEndpoints(endpoints =>
                            {
                                endpoints
                                    .MapGet("/test", () => "Test responce")
                                    .RequireRateLimiting("policy2", Region.US);
                            });

                            app.UseRateLimiter();
                        });
                })
                .StartAsync();

            var client = host.GetTestClient();

            client.DefaultRequestHeaders.Add("Authorization", "test-token");
            client.DefaultRequestHeaders.Add("Region", "EU");

            var response1 = await client.GetAsync("/test");
            var response2 = await client.GetAsync("/test");

            Assert.True(response1.IsSuccessStatusCode);
            Assert.True(response2.StatusCode == System.Net.HttpStatusCode.TooManyRequests);
        }
    }
}
