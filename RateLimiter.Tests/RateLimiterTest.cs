using NUnit.Framework;
using RateLimiter.Models;
using RateLimiter.Rules;
using RateLimiter.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace RateLimiter.Tests;

[TestFixture]
public class RateLimiterTest
{
    //Pulling the rules from appsettings to demonstrate
    private const string _appsettingsFilePath = "../../../appsettings.json";
    private string _rulesBySourceJsonString = "";
    [SetUp]
    public void Setup()
    {
        string appSettingsString = File.ReadAllText(_appsettingsFilePath);
        var jsonDocument = JsonDocument.Parse(appSettingsString);
        _rulesBySourceJsonString = jsonDocument.RootElement.GetProperty("RateLimiterConfig").GetRawText();
    }
	[Test]
	public void Example()
	{
        RateLimiterService service = new RateLimiterService(_rulesBySourceJsonString);

        List<RequestModel> requestModels = new List<RequestModel>()
        {
            new RequestModel()
            {
                TimeRequested = DateTime.Now.AddMinutes(-10),
                Source = "CheckoutEndPoint",
                Location = "Location1",
                UserID = "User1"
            },
            new RequestModel()
            {
                TimeRequested = DateTime.Now.AddMinutes(-5),
                Source = "CheckoutEndPoint",
                Location = "Location1",
                UserID = "User2"
            },

        };

        foreach(var request in requestModels)
        {
            Assert.IsTrue(service.ValidateRequest(request));
        }
    }
}