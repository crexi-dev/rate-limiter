using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using Moq;

namespace RateLimiter.Tests.Helpers;

public class ActionFilterHelper
{
    public static ActionExecutingContext CreateActionExecutingContext(string token, string actionName)
    {
        var httpContext = new DefaultHttpContext
        {
            Request =
            {
                Headers =
                {
                    new KeyValuePair<string, StringValues>("AccessToken", token)
                }
            }
        };

        var actionContext = new ActionContext
        {
            HttpContext = httpContext,
            RouteData = new RouteData(),
            ActionDescriptor = new ActionDescriptor
            {
                DisplayName = actionName
            }
        };

        var actionExecutingContext = new ActionExecutingContext(
            actionContext,
            new List<IFilterMetadata>(),
            new Dictionary<string, object>(),
            new Mock<Controller>().Object
        );

        return actionExecutingContext;
    }
}