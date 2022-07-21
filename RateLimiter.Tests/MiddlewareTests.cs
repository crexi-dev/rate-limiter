using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using RateLimiter.Middlewares;
using System;
using System.Net;
using System.Threading.Tasks;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class MiddlewareTests
    {
        private TestServer server;

        [SetUp]
        public async Task Setup()
        {
            var host = await new HostBuilder()
                .ConfigureWebHost(webBuilder =>
                {
                    webBuilder
                        .UseTestServer()
                        .ConfigureServices(services =>
                        {
                            services.AddMyServices();
                        })
                        .Configure(app =>
                        {
                            app.UseMiddleware<RateLimiterMiddleware>();
                        });
                })
                .StartAsync();

            
            server = host.GetTestServer();
            server.BaseAddress = new Uri("https://example.com/A/Path/");
        }

        [TearDown]
        public void TearDown()
        {
            if (server is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        [Test]
        public async Task MiddlewareTest_ReturnsNotFoundForRequest()
        {
            //Arrange
            IPAddress fakeIpAddress = IPAddress.Parse("127.168.1.32");

            //Act
            var context = await server.SendAsync(c =>
            {
                c.Request.Method = HttpMethods.Post;
                c.Connection.RemoteIpAddress = fakeIpAddress;  
            });

            //Assert
            Assert.AreEqual(HttpStatusCode.NotFound, (HttpStatusCode)context.Response.StatusCode);
        }

        [Test]
        public async Task MiddlewareTest_MultipleRequestForUS_TooManyRequests()
        {
            //Arrange
            IPAddress fakeIpAddress = IPAddress.Parse("127.168.1.32");


            //Act
            HttpContext context = await server.SendAsync(c =>
            {
                c.Request.Method = HttpMethods.Post;
                c.Connection.RemoteIpAddress = fakeIpAddress;
            }); 

            for (int i = 0; i < 10; i++)
            {
                context = await server.SendAsync(c =>
                {
                    c.Request.Method = HttpMethods.Post;
                    c.Connection.RemoteIpAddress = fakeIpAddress;
                });
            }
            
            //Assert
            Assert.AreEqual(HttpStatusCode.TooManyRequests, (HttpStatusCode)context.Response.StatusCode);
        }
    }
}
