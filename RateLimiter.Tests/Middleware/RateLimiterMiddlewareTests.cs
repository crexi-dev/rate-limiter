using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using RateLimiter.Processor;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace RateLimiter.Tests.Middleware
{
    [TestFixture]
    public class RateLimiterMiddlewareTests
    {

        [TestCase]
        public async Task RateLimiterMiddleware_Success()
        {
            var requestDelegate = new Mock<RequestDelegate>();
            var context = new Mock<HttpContext>();
            var processor = new Mock<IRateLimitProcessor>();
            processor.Setup(x => x.Process(context.Object))
            .Returns(true);

            var middleware = new RateLimiterMiddleware(requestDelegate.Object, processor.Object);

            await middleware.InvokeAsync(context.Object);

            requestDelegate.Verify(next => next(context.Object));
        }

        [TestCase]
        public async Task RateLimiterMiddleware_Fail()
        {
            var requestDelegate = new Mock<RequestDelegate>();
            var httpResponse = new Mock<HttpResponse>();
            httpResponse.SetupAllProperties();
            var context = new Mock<HttpContext>();
            context.Setup(x => x.Response)
                .Returns(httpResponse.Object);

            var processor = new Mock<IRateLimitProcessor>();
            processor.Setup(x => x.Process(context.Object))
            .Returns(false);

            var middleware = new RateLimiterMiddleware(requestDelegate.Object, processor.Object);

            await middleware.InvokeAsync(context.Object);

            Assert.That(httpResponse.Object.StatusCode, Is.EqualTo((int)HttpStatusCode.TooManyRequests));
        }
    }
}
