**Rate-limiting pattern**

Based on RateLimiting service from [ocelot repo](https://github.com/ThreeMammals/Ocelot).
You can see an example of using in the [SimpleSample](https://github.com/DAKnyazev/rate-limiter/tree/master/Samples/SimpleSample) project.

## Usage

To access rate limiting counter you need to resolve service 'IRateLimitingService'

### With memory cache
```csharp
    .AddRateLimitingServiceWithMemoryCache();
```

### With distributed cache
```csharp
    .AddRateLimitingServiceWithDistributedCache();
```

### With custom cache
```csharp
    .AddRateLimitingServiceCore()
    .AddSingleton<IRateLimitStorageService, CustomRateLimitStorageService>();
```
You need to implement IRateLimitStorageService by yourself.
