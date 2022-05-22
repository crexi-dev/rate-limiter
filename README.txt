This RateLimiter is a custom .net core middleware that should be implemented in startup.cs file
For Showcase it is implemented on TestApi project in the repository. 

There are also Integration tests written inside RateLimiterTests project 
to demonstrate it's functionality on TestApi.


WARNING: TestApi has no authorization implemented so you cannot test RateLimiter  through  swagger.
Use Postman(or similar) and send bearer authorization tokens in header to test RateLimiter. 
See  "ApiEndpointTests.cs". as an example(test tokens are used to test functionality of Rate Limiter). 

NOTE: to run those integration tests, you first need to run TestApi without debugging and then run those tests.
(you might have to change main URL in tests file in case your system changes the url of TestApi)

Rate Limiter has configurable rules, that can be configured through appsettings.json of api that is implementing it.
Currently it has two possible rules: 
 1) Limit Per Timespan Rule -  limiting requests per timespan using token bucket algorithm.
 2) Interval Rule - enforcing a time interval between requests.

rules can be added to individual ednpoints and controllers. default rule must be implemented.
rules are prioritized this way:

1)specific endpoint rules override controller rules.
2) controler rules override default rule
3) default rule works for api endpoints that have neither controller rule, nor endpoint rule.

you can see tests demonstrating every of these caeses in "ApiEndpointTests.cs".

one endpoint can have many rules.

this project uses static class with static dictionary as in memory database. However, repository pattern is used
so one can create new database implementations if necessary.

data is organized this way:

clients are distinguished by authorization tokens.
one token can have several endpoint rules attached to it.
one endpoint can have several rules attached to it. 

to configure rate limiter use appsettings.json inside TestApi as an example.

WARNING: rule names should be exactly the same as class names for the said rules. RateLimiter is creating 
instances for those rules through refleciton and it will fail if those names are not exact match.

default rule should also have endpoint specified. mostly it should be the url of api as in TestApi.

period and limit fields are mandatory for Limit Per Timespan Rule.

only period field is mandatory for Interval Rule.


to implement it first configure settings for rate limiter using:
 services.Configure<RateLimiterConfigModel>(Configuration.GetSection("RateLimiter"));

then use it as middleware in  Configure Method:
 app.UseMiddleware<RateLimiterMiddleWare>();

You can use Startup.cs in TestApi as an example. 
