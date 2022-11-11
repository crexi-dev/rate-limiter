namespace RateLimiter.AspNetCore;

public class RateLimitingOptions
{
    public Dictionary<string, IParameterProvider> ParameterProviders { get; } = new(StringComparer.OrdinalIgnoreCase);
}