using Microsoft.Extensions.Configuration;
using RuleLimiterTask;
using RuleLimiterTask.Rules;
using RuleLimiterTask.Rules.Settings;
using System.Reflection;

var builder = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location))
                .AddJsonFile("config.json", optional: false);

IConfiguration config = builder.Build();

// requests
var userRequest1 = new UserRequest(Region.EU, 1);
var userRequest2 = new UserRequest(Region.EU, 1);
var userRequest3 = new UserRequest(Region.EU, 1);
var userRequest4 = new UserRequest(Region.EU, 1);
var userRequest5 = new UserRequest(Region.EU, 1);
var userRequest6 = new UserRequest(Region.EU, 1);
var userRequest7 = new UserRequest(Region.EU, 1);

// rule settings
var requestPerTimespanSettings = config.GetSection("RequestPerTimespan").Get<RequestPerTimespanSettings>();
var timespanSinceLastCallSettings = config.GetSection("TimespanPassedSinceLastCall").Get<TimespanSinceLastCallSettings>();

// cache
var cache = new CacheService();

// rules
var requestPerTimespanRule = new RequestPerTimespanRule(requestPerTimespanSettings);
var timespanSinceLastCallRule = new TimespanSinceLastCallRule(timespanSinceLastCallSettings);

// resources
var resource = new Resource();
resource.ApplyRule(requestPerTimespanRule);
resource.ApplyRule(timespanSinceLastCallRule);

userRequest1.RequestAccess(resource, cache);
await Task.Delay(10);
userRequest2.RequestAccess(resource, cache);
await Task.Delay(10);
userRequest3.RequestAccess(resource, cache);
await Task.Delay(10);
userRequest4.RequestAccess(resource, cache);
await Task.Delay(10);
userRequest5.RequestAccess(resource, cache);
await Task.Delay(10);
userRequest6.RequestAccess(resource, cache);
await Task.Delay(10);
userRequest7.RequestAccess(resource, cache);

Console.WriteLine($"user request 1 is {userRequest1.State}");
Console.WriteLine($"user request 2 is {userRequest2.State}");
Console.WriteLine($"user request 3 is {userRequest3.State}");
Console.WriteLine($"user request 4 is {userRequest4.State}");
Console.WriteLine($"user request 5 is {userRequest5.State}");
Console.WriteLine($"user request 6 is {userRequest6.State}");
Console.WriteLine($"user request 7 is {userRequest7.State}");