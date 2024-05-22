namespace RateLimiter.Tests;

using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using NUnit.Framework;

using Rules;

[TestFixture]
public class TimeSinceLastCallAttributeTest
{
    [Test]
    public async Task TestTimeSinceLastCallWithoutCountry_OneRequest()
    {
        using var host = await new HostBuilder()
                             .ConfigureWebHost(webBuilder =>
                                 {
                                     webBuilder
                                         .UseTestServer()
                                         .ConfigureServices(services =>
                                             {
                                                 services.AddRouting();
                                                 services.AddDistributedMemoryCache();
                                             })
                                         .Configure(app =>
                                             {
                                                 app.UseRouting();
                                                 app.UseMiddleware<RateLimitMiddleware>();
                                                 app.UseEndpoints(endpoints =>
                                                     {
                                                         endpoints.MapGet("/hello", () => "Hello World!")
                                                             .WithMetadata(new TimeSinceLastCallAttribute(1));
                                                     });
                                             });
                                 })
                             .StartAsync();

        var client = host.GetTestClient();

        var response = await client.GetAsync("/hello");

        Assert.True(response.IsSuccessStatusCode);
        var responseBody = await response.Content.ReadAsStringAsync();
        Assert.AreEqual("Hello World!", responseBody);
    }

    [Test]
    public async Task TestTimeSinceLastCallWithoutCountry_TwoRequestsWithoutDelay()
    {
        using var host = await new HostBuilder()
                             .ConfigureWebHost(webBuilder =>
                                 {
                                     webBuilder
                                         .UseTestServer()
                                         .ConfigureServices(services =>
                                             {
                                                 services.AddRouting();
                                                 services.AddDistributedMemoryCache();
                                             })
                                         .Configure(app =>
                                             {
                                                 app.UseRouting();
                                                 app.UseMiddleware<RateLimitMiddleware>();
                                                 app.UseEndpoints(endpoints =>
                                                     {
                                                         endpoints.MapGet("/hello", () => "Hello World!")
                                                             .WithMetadata(new TimeSinceLastCallAttribute(10));
                                                     });
                                             });
                                 })
                             .StartAsync();

        var client = host.GetTestClient();
        var results = new List<HttpResponseMessage>();

        for (var i = 0; i < 2; i++)
        {
            results.Add(await client.GetAsync("/hello"));
        }

        Assert.AreEqual(results.Count(x => !x.IsSuccessStatusCode), 1);
    }

    [Test]
    public async Task TestTimeSinceLastCallWithoutCountry_TwoRequestsWithDelay()
    {
        using var host = await new HostBuilder()
                             .ConfigureWebHost(webBuilder =>
                                 {
                                     webBuilder
                                         .UseTestServer()
                                         .ConfigureServices(services =>
                                             {
                                                 services.AddRouting();
                                                 services.AddDistributedMemoryCache();
                                             })
                                         .Configure(app =>
                                             {
                                                 app.UseRouting();
                                                 app.UseMiddleware<RateLimitMiddleware>();
                                                 app.UseEndpoints(endpoints =>
                                                     {
                                                         endpoints.MapGet("/hello", () => "Hello World!")
                                                             .WithMetadata(new TimeSinceLastCallAttribute(1));
                                                     });
                                             });
                                 })
                             .StartAsync();

        var client = host.GetTestClient();
        var results = new List<HttpResponseMessage>();

        for (var i = 0; i < 2; i++)
        {
            results.Add(await client.GetAsync("/hello"));
            await Task.Delay(1500);
        }

        Assert.AreEqual(results.Count(x => x.IsSuccessStatusCode), 2);
    }
}