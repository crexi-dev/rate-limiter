using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using NUnit.Framework;

namespace RateLimiter.Filters.Tests
{
    [TestFixture]
    public class AuthorizationFilterTests
    {
        private const string _accessToken = "12345abcdef";

        [Test]
        public void TestSuccessfulAuthorization()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Authorization"] = $"Bearer {_accessToken}";

            var fakeActionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

            var fakeAuthFilterContext = new AuthorizationFilterContext(fakeActionContext, new List<IFilterMetadata>());

            //Act
            var authorizationFilter = new AuthorizationFilter();
            authorizationFilter.OnAuthorization(fakeAuthFilterContext);

            //Assert
            fakeAuthFilterContext.Result.Should().BeNull();
        }

        [Test]
        public void TestFailedAuthorization()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();

            var fakeActionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

            var fakeAuthFilterContext = new AuthorizationFilterContext(fakeActionContext, new List<IFilterMetadata>());

            //Act
            var authorizationFilter = new AuthorizationFilter();
            authorizationFilter.OnAuthorization(fakeAuthFilterContext);

            //Assert
            fakeAuthFilterContext.Result.Should().BeOfType(typeof(UnauthorizedResult));
        }
    }
}