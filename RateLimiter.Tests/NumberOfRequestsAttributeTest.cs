using NUnit.Framework;

namespace RateLimiter.Tests;

using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Rules;

using Storage;

[TestFixture]
public class NumberOfRequestsAttributeTest
{
    [Test]
    public async Task TestNumberOfRequestsWithoutCountry_OneRequest()
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
                        .WithMetadata(new NumberOfRequestsAttribute(60, 10));
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
    public async Task TestNumberOfRequestsWithUSCountry_TwentyRequests()
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
                        .WithMetadata(new NumberOfRequestsAttribute(60, 10, "EU"), new NumberOfRequestsAttribute(5, 5, "US"));
                });
            });
        })
        .StartAsync();

        var client = host.GetTestClient();
        var results = new List<HttpResponseMessage>();

        CountryCodeStorage.CountryCode = "US";

        for (var i = 0; i < 20; i++)
        {
            results.Add(await client.GetAsync("/hello"));
        }

        Assert.AreEqual(results.Count(x => x.IsSuccessStatusCode), 5);
        Assert.AreEqual(results.Where(x => !x.IsSuccessStatusCode).Select(x => x.StatusCode).Distinct().Single(), HttpStatusCode.TooManyRequests);
    }

    [Test]
    public async Task TestNumberOfRequestsWithEUCountry_TwentyRequests()
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
                        .WithMetadata(new NumberOfRequestsAttribute(60, 10, "EU"), new NumberOfRequestsAttribute(5, 5, "US"));
                });
            });
        })
        .StartAsync();

        var client = host.GetTestClient();
        var results = new List<HttpResponseMessage>();

        CountryCodeStorage.CountryCode = "EU";

        for (var i = 0; i < 20; i++)
        {
            results.Add(await client.GetAsync("/hello"));
        }

        Assert.AreEqual(results.Count(x => x.IsSuccessStatusCode), 10);
        Assert.AreEqual(results.Where(x => !x.IsSuccessStatusCode).Select(x => x.StatusCode).Distinct().Single(), HttpStatusCode.TooManyRequests);
    }

    [Test]
    public async Task TestNumberOfRequestsWithoutCountry_TwentyRequests()
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
                            .WithMetadata(new NumberOfRequestsAttribute(60, 10));
                    });
                });
            })
        .StartAsync();

        var client = host.GetTestClient();
        var results = new List<HttpResponseMessage>();

        for (var i = 0; i < 20; i++)
        {
            results.Add(await client.GetAsync("/hello"));
        }

        Assert.AreEqual(results.Count(x => x.IsSuccessStatusCode), 10);
        Assert.AreEqual(results.Where(x => !x.IsSuccessStatusCode).Select(x => x.StatusCode).Distinct().Single(), HttpStatusCode.TooManyRequests);
    }

    [Test]
    public async Task TestNumberOfRequestsWithoutCountry_TwoRequestsWithDelay()
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
                        .WithMetadata(new NumberOfRequestsAttribute(1, 3));
                });
            });
        })
        .StartAsync();

        var client = host.GetTestClient();
        var results = new List<HttpResponseMessage>();

        for (var i = 0; i < 2; i++)
        {
            results.Add(await client.GetAsync("/hello"));
            await Task.Delay(1000);
        }

        Assert.AreEqual(results.Count(x => x.IsSuccessStatusCode), 2);
    }
}