# Rate Limiter
Rate limiting is based on auth token and region. For simplicity both token and region are expected in request headers.

 _Authorization_ - token;
 _Region_ - region;

## How To configure Rate Limiter
To add rate limiting to an app the following is required:
1. Register Rate Limiter policies.
The following Rate Limiter types are supported:
- TimespanLimiter
- FixedWindowLimiter

2. Register Rate Limiter processor and it's dependencies using _AddRateLimiter()_ extension.

3. Add middlware using _UseRateLimiter()_ extension method.

See configuration example:
~~~
// register Rate Limiter policy
services.AddTimespanRateLimiter(
    policy: "policy1",
    region: Region.EU,
    configureOptions: x =>
    {
        x.Window = TimeSpan.FromSeconds(3);
    });
    
// register Rate Limiter policy
services.AddFixedWindowRateLimiter(
    policy: "policy1",
    region: Region.EU,
    configureOptions: x =>
    {
        x.Window = TimeSpan.FromSeconds(3);
        x.Limit = 3;
    });

// register Rate Limiter dependencies
services.AddRateLimiter();

...

// Register Middlware
app.UseRateLimiter();
~~~

The last step - assign policies to endpoints. Two approaches are supported.

#### Controllers
Eeach method that should follow rate limiter rules must be marked by an attribute _EnableRateLimitingAttribute_.
Multiple policies are supported per each region.

See configuration example:
~~~
[EnableRateLimiting(Region.EU, "policy1")]
[EnableRateLimiting(Region.US, "policy2")]
public IEnumerable<WeatherForecast> Get()
{
    // logic here
}
~~~

#### Minimal API
_RequireRateLimiting_ extension method must be used to assign policies to each endpoint.
Multiple policies are supported per each region.

See configuration example:
~~~
app.UseEndpoints(endpoints =>
{
    endpoints.MapGet("/test", () => "Test responce")
        .RequireRateLimiting("policy2", Region.US)
        .RequireRateLimiting("policy1", Region.EU);
});
~~~

## Add new Rate Limiter types
In order to add new Rate Limiter you need to do the following:
1. Add new Rate Limiter Options. It must implement _ILimiterOptions_ interface.
2. Add new Rate Limiter. It must implement _ILimiter_ interface.
3. Add new extension method to _DependencyInjection.cs_, so there is an ability to configure newly created Rate Limiter.



P.S.
This solution is not ideal, but I hope the idea of what I tried to achive is clear 😊