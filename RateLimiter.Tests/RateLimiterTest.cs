using NUnit.Framework;
using Moq;
using RateLimiter.Limiter;
using ResourceApi.Helpers;
using System;
using RateLimiter.Interfaces;
using System.Collections.Generic;
using RateLimiter.Rules;

namespace RateLimiter.Tests;

[TestFixture]
public class RateLimiterTest
{

    [Test]
    public void GetImageResourceNotValid()
    {
        var img = String.Empty;

        var resourceLimitter = CreateRateLimiter.ResourceLimitter;

        var isValid = resourceLimitter.Validate(img);

        Assert.IsFalse(isValid);
    }

    [Test]
    public void GetImageResourceIsValid()
    {
        var img = "img1.jpg";

        var resourceLimitter = CreateRateLimiter.ResourceLimitter;

        var isValid = resourceLimitter.Validate(img);

        Assert.IsTrue(isValid);
    }

    [Test]
    public void GetFileResourceNotValid()
    {
        var fileName = String.Empty;

        IAllowRequest allowRequestPerTimespan;
        IAllowRequest allowTimespanSinceLastCall;
        allowRequestPerTimespan = new RequestPerTimespan(10, TimeSpan.FromSeconds(60));
        allowTimespanSinceLastCall = new TimespanSinceLastCall(TimeSpan.FromSeconds(40));

        List<IAllowRequest> rules = new List<IAllowRequest>();
        rules.Add(allowRequestPerTimespan);
        rules.Add(allowTimespanSinceLastCall);

        var mixedResourceLimitter = new MixedResourceLimitter(rules);

        var isValid = mixedResourceLimitter.Validate(fileName);

        Assert.IsFalse(isValid);
    }

    [Test]
    public void GetFileResourceIsValid()
    {
        var fileName = "file.doc";

        IAllowRequest allowRequestPerTimespan;
        IAllowRequest allowTimespanSinceLastCall;
        allowRequestPerTimespan = new RequestPerTimespan(10, TimeSpan.FromSeconds(60));
        allowTimespanSinceLastCall = new TimespanSinceLastCall(TimeSpan.FromSeconds(40));

        List<IAllowRequest> rules = new List<IAllowRequest>();
        rules.Add(allowRequestPerTimespan);
        rules.Add(allowTimespanSinceLastCall);

        var mixedResourceLimitter = new MixedResourceLimitter(rules);        

        var isValid = mixedResourceLimitter.Validate(fileName);


        Assert.IsTrue(isValid);
    }

}