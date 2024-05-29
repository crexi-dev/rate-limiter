## Rate Limiter - A Library for Managing Access Restrictions

**Description:**

Rate Limiter is a .NET library that provides functionality for managing access restrictions to resources. The library allows you to configure rate limiting rules and check client access to resources based on these rules.

**Functionality:**

* **Rate limiting rules:** The library supports the following rule types:
    * **TimeSinceLastCallRule:** Limits the number of requests from a single client within a given time interval.
    * **XRequestsPerTimespanRule:** Limits the number of requests from a single client within a specified period of time.
    * **Custom rules:** You can create your own rules by implementing the `IRateLimitingRule` interface.
* **Access verification:** The library provides an `ValidateRequestAsync` method that checks client access to a resource based on the configured rules.
* **Logging:** Built-in logging support allows you to track events related to rate limiting for debugging and monitoring purposes.
* **Testing:** The library comes with unit tests, ensuring the correct operation of Rate Limiter.

**Usage:**

**1. Configuring RuleProvider:**

```C#
// In Startup.cs or your configuration setup
public void ConfigureServices(IServiceCollection services)
{
    // ... other services

    services.Configure<RuleProviderOptions>(options =>
    {
        options.Rules = new Dictionary<string, List<IRateLimitingRule>>
        {
            { "US", new List<IRateLimitingRule>
                {
                    new TimeSinceLastCallRule(TimeSpan.FromSeconds(1), new DateTimeWrapper()),
                    new XRequestsPerTimespanRule(10, TimeSpan.FromMinutes(1), new DateTimeWrapper())
                }
            },
            { "EU", new List<IRateLimitingRule>
                {
                    new TimeSinceLastCallRule(TimeSpan.FromSeconds(2), new DateTimeWrapper())
                }
            },
            // ... add rules for other regions
        };
    });

    services.AddSingletone<IRateLimitingService, RateLimitingService>();
    services.AddSingletone<IRuleProvider, RuleProvider>();
}
```

**2. Using RateLimitingService for comprehensive validation:**

```C#
// In your controller
public class MyController : ControllerBase
{
    private readonly IRateLimitingService _rateLimitingService;

    public MyController(IRateLimitingService rateLimitingService)
    {
        _rateLimitingService = rateLimitingService;
    }

    [HttpGet]
    public async Task<IActionResult> MyAction()
    {
        // Check access using the registered rules
        var result = await _rateLimitingService.IsRequestAllowedAsync("resource", "US-token"); // or EU-token

        if (result.IsAllowed)
        {
            // Allow access to the resource
            return Ok(); // or other appropriate action
        }
        else
        {
            // Deny access to the resource, display errors (result.Errors)
            return StatusCode(429, result.Errors); // or other appropriate error response
        }
    }
}
```

**3. Using individual rules for validation:**

```C#
// In your controller or service
var ruleProvider = container.Resolve<IRuleProvider>();
var rules = ruleProvider.GetRulesForResource("resource", "US"); // or "EU"

// Iterate through the rules and check access for each
foreach (var rule in rules)
{
    if (!rule.IsRequestAllowed("US-token")) // or "EU-token"
    {
        // Block access, handle errors
        return StatusCode(429, rule.GetError()); // or other appropriate error response
    }
}

// Allow access
return Ok(); // or other appropriate action
```

**Benefits of Rate Limiter:**

* **Flexibility:**  The ability to use various rate limiting rules to control access to resources.
* **Extensibility:**  Support for creating custom rate limiting rules.
* **Testability:**  The presence of unit tests ensures the library's correct operation.
* **Logging:**  Easy debugging and monitoring through built-in logging.

