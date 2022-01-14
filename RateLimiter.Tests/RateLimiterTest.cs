using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using RateLimiter.Constants;
using RateLimiter.Models;
using RateLimiter.Models.Config;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTest
    {
        [TestCase("key1", HttpStatusCode.NotFound)]
        [TestCase("key3", HttpStatusCode.Unauthorized)]
        [TestCase(null, HttpStatusCode.Unauthorized)]
        public async Task Validate_ClientKeyAllowedIdentifier(string key, HttpStatusCode expectedResult)
        {
            // Arrange
            RateLimitConfiguration rateLimitConfiguration = new()
            {
                ClientKeyConfiguration = new ClientConfigKeyConfiguration
                {
                    AllowedIdentifiers = new[] { "key1", "key2" }
                }
            };

            HttpClient client = await GetClient(rateLimitConfiguration);
            Uri uri = new(client.BaseAddress!.ToString());

            // Act
            HttpResponseMessage response;
            using (HttpRequestMessage requestMessage = new(HttpMethod.Get, uri))
            {
                requestMessage.Headers.Add(Headers.ClientKey, key);
                response = await client.SendAsync(requestMessage);
            }

            // Assert
            Assert.AreEqual(expectedResult, response.StatusCode);
        }

        [TestCase("ip1", "key1", HttpStatusCode.NotFound)]
        [TestCase("ip2", "key3", HttpStatusCode.Unauthorized)]
        [TestCase("ip5", "key", HttpStatusCode.Unauthorized)]
        [TestCase(null, null, HttpStatusCode.Unauthorized)]
        public async Task Validate_IpAndClientAllowedIdentifier(string ip, string key, HttpStatusCode expectedResult)
        {
            // Arrange
            RateLimitConfiguration rateLimitConfiguration = new()
            {
                IpConfiguration = new IpConfiguration
                {
                    AllowedIdentifiers = new[] { "ip1", "ip2" }
                },
                ClientKeyConfiguration = new ClientConfigKeyConfiguration
                {
                    AllowedIdentifiers = new[] { "key1", "key2" }
                }
            };
            HttpClient client = await GetClient(rateLimitConfiguration);
            Uri uri = new(client.BaseAddress!.ToString());

            // Act
            HttpResponseMessage response;
            using (HttpRequestMessage requestMessage = new(HttpMethod.Get, uri))
            {
                requestMessage.Headers.Host = ip;
                requestMessage.Headers.Add(Headers.ClientKey, key);
                response = await client.SendAsync(requestMessage);
            }

            // Assert
            Assert.AreEqual(expectedResult, response.StatusCode);
        }

        [TestCase(1, 0)]
        public async Task Should_ThrottleMultipleSimultaneousRequestsFromOneClient_OnlyTwoRequestPerMinuteAllowed(int count,
            int expectedRejectedRequests)
        {
            // Arrange
            RateLimitConfiguration rateLimitConfiguration = new()
            {
                ClientKeyConfiguration = new ClientConfigKeyConfiguration
                {
                    Policies = new List<Policy>
                    {
                        new() {Count = 2, Path = "/Get/", Timeout = TimeSpan.FromMinutes(1)}
                    }
                }
            };
            HttpClient client = await GetClient(rateLimitConfiguration);
            Uri uri = new(client.BaseAddress + "Get/");

            // Act
            IEnumerable<Task<HttpResponseMessage>> requests = Enumerable.Range(0, count)
                .Select(_ => client.SendAsync(new HttpRequestMessage(HttpMethod.Get, uri)));
            HttpResponseMessage[] results = await Task.WhenAll(requests);

            // Assert
            Assert.AreEqual(expectedRejectedRequests, results.Count(r => r.StatusCode == HttpStatusCode.TooManyRequests));
        }
        
        [TestCase(2, 0)]
        public async Task Should_ThrottleMultipleSimultaneousRequestsFromClients_OnlyTwoRequestPerMinuteAllowed(int requestsByClient,
            int expectedRejectedRequests)
        {
            // Arrange
            RateLimitConfiguration rateLimitConfiguration = new()
            {
                ClientKeyConfiguration = new ClientConfigKeyConfiguration
                {
                    Policies = new List<Policy>
                    {
                        new() {Count = 2, Path = "/Get/", Timeout = TimeSpan.FromMinutes(1)}
                    }
                }
            };
            HttpClient client = await GetClient(rateLimitConfiguration);
            Uri uri = new(client.BaseAddress + "Get/");

            // Act
            IEnumerable<Task<HttpResponseMessage>> client1Requests = Enumerable.Range(0, requestsByClient).Select(_ => SendClientMessageAsync(uri, client, "client1"));
            IEnumerable<Task<HttpResponseMessage>> client2Requests = Enumerable.Range(0, requestsByClient).Select(_ => SendClientMessageAsync(uri, client, "client2"));
            IEnumerable<Task<HttpResponseMessage>> allRequests = client1Requests.Concat(client2Requests);
            HttpResponseMessage[] results = await Task.WhenAll(allRequests);

            // Assert
            Assert.AreEqual(expectedRejectedRequests, results.Count(r => r.StatusCode == HttpStatusCode.TooManyRequests));
        }

        [TestCase(2, 0)]
        public async Task Should_ThrottleMultipleSimultaneousRequestsFromClients_OnlyTwoRequestsPerMinuteAllowedPerResource(int requestsByClient,
            int expectedRejectedRequests)
        {
            // Arrange
            RateLimitConfiguration rateLimitConfiguration = new()
            {
                ClientKeyConfiguration = new ClientConfigKeyConfiguration
                {
                    Policies = new List<Policy>
                    {
                        new() {Count = 2, Path = "/Del/", Timeout = TimeSpan.FromMinutes(1)},
                        new() {Count = 2, Path = "/Post/", Timeout = TimeSpan.FromMinutes(1)}
                    }
                }
            };
            HttpClient client = await GetClient(rateLimitConfiguration);
            Uri uri1 = new(client.BaseAddress + "Del/");
            Uri uri2 = new(client.BaseAddress + "Post/");

            // Act
            IEnumerable<Task<HttpResponseMessage>> client1Requests = Enumerable.Range(0, requestsByClient).Select(_ => SendClientMessageAsync(uri1, client, "client1"));
            IEnumerable<Task<HttpResponseMessage>> client2Requests = Enumerable.Range(0, requestsByClient).Select(_ => SendClientMessageAsync(uri2, client, "client2"));
            IEnumerable<Task<HttpResponseMessage>> allRequests = client1Requests.Concat(client2Requests);
            HttpResponseMessage[] results = await Task.WhenAll(allRequests);

            // Assert
            Assert.AreEqual(expectedRejectedRequests, results.Count(r => r.StatusCode == HttpStatusCode.TooManyRequests));
        }

        private static Task<HttpResponseMessage> SendClientMessageAsync(Uri uri, HttpClient client, string clientName)
        {
            HttpRequestMessage message = new(HttpMethod.Get, uri);
            message.Headers.Add(Headers.ClientKey, clientName);
            return client.SendAsync(message);
        }

        private static async Task<HttpClient> GetClient(RateLimitConfiguration configuration)
        {
            IHost host = await CreateHost(configuration);
            return host.GetTestClient();
        }

        private static Task<IHost> CreateHost(RateLimitConfiguration configuration)
        {
            return new HostBuilder()
                .ConfigureWebHost(webBuilder =>
                {
                    webBuilder
                        .UseTestServer()
                        .Configure(app =>
                        {
                            app.UseMiddleware<Models.Middleware.RateLimiter>(configuration);
                        });
                })
                .StartAsync();
        }
    }
}
