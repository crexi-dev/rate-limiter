using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Primitives;
using Moq;
using RateLimiter.Builders;
using RateLimiter.Middleware;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace RateLimiter.Tests
{
    public class RateLimitterMiddlewareTests
    {
        [Fact]
        public async Task SimpleCall_WithDefaultHttpContext_Succeeds()
        {
            var mockRequestDelegate = new Mock<RequestDelegate>();
            var mockRulesProvider = new Mock<IRulesProvider>();
            var mockClientRequestResolver = new Mock<IClientRequestResolver>();
            var mockRuleRunnersBuilder = new Mock<IRuleRunnersBuilder>();
            var requestFeature = new HttpRequestFeature();
            var features = new FeatureCollection();

            features.Set<IHttpRequestFeature>(requestFeature);
            var httpContext = new DefaultHttpContext(features);
            var authServiceMock = new Mock<IAuthenticationService>();
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(_ => _.GetService(It.IsAny<Type>()))
                .Returns(authServiceMock.Object);

            httpContext.RequestServices = serviceProviderMock.Object;

            var middleware = new RateLimitterMiddleware(mockRequestDelegate.Object, mockRulesProvider.Object, mockClientRequestResolver.Object, mockRuleRunnersBuilder.Object);

            await middleware.InvokeAsync(httpContext);
        }
    }
}
