using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using NSubstitute;
using NUnit.Framework;
using RateLimiter.Rules;

namespace RateLimiter.Tests;

[TestFixture]
public class RateLimitingMiddlewareTests
{
    private RequestDelegate _next;
    private HttpContext _httpContext;
    private ControllerActionDescriptor _actionDescriptor;
    private IServiceProvider _serviceProvider;

    [SetUp]
    public void SetUp()
    {
        _next = Substitute.For<RequestDelegate>();
        _httpContext = new DefaultHttpContext();
        _serviceProvider = Substitute.For<IServiceProvider>();
        _httpContext.RequestServices = _serviceProvider;

        _actionDescriptor = new ControllerActionDescriptor
        {
            ControllerTypeInfo = typeof(DummyController).GetTypeInfo(),
            MethodInfo = typeof(DummyController).GetMethod(nameof(DummyController.DummyAction))!
        };
        var endpoint = new Endpoint(ctx => Task.CompletedTask, new EndpointMetadataCollection(_actionDescriptor), "TestEndpoint");
        _httpContext.SetEndpoint(endpoint);
    }
    
    [TestCase(true, false, false)]
    [TestCase(false, true, false)]
    [TestCase(true, true, true)]
    [TestCase(null, true, true)]
    [TestCase(true, null, true)]
    [TestCase(null, false, false)]
    [TestCase(false, null, false)]
    [TestCase(null, null, true)]
    public async Task InvokeAsync_AppliesRulesCorrectly(bool? controllerRule, bool? actionRule, bool expectedResult)
    {
        SetRuleForController(controllerRule);
        SetRuleForAction(actionRule);

        var middleware = new RateLimitingMiddleware(_next);
        await middleware.InvokeAsync(_httpContext);

        if (expectedResult)
        {
            await _next.Received(1).Invoke(_httpContext);
        }
        else
        {
            _httpContext.Response.StatusCode.Should().Be(StatusCodes.Status429TooManyRequests);
            await _next.DidNotReceive().Invoke(_httpContext);
        }
    }

    private void SetRuleForController(bool? rule)
    {
        if (!rule.HasValue)
        {
            return;
        }
        
        var controllerInfo = Substitute.For<TypeInfo>();
        controllerInfo.GetCustomAttributes(typeof(RuleAttribute), true).Returns([new TestRule(rule.Value)]);

        _actionDescriptor.ControllerTypeInfo = controllerInfo;
    }
    
    private void SetRuleForAction(bool? rule)
    {
        if (!rule.HasValue)
        {
            return;
        }
        
        var methodInfo = Substitute.For<MethodInfo>();
        methodInfo.GetCustomAttributes(typeof(RuleAttribute), true).Returns([new TestRule(rule.Value)]);

        _actionDescriptor.MethodInfo = methodInfo;
    }

    private class TestRule(bool result) : RuleAttribute
    {
        public override Task<bool> IsAllowedAsync(IServiceProvider sp, CancellationToken ct)
        {
            return Task.FromResult(result);
        }
    }

    private class DummyController
    {
        public void DummyAction()
        {
        }
    }
}
