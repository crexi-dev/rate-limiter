namespace RateLimiter.AspNetCore.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class IgnoreRateLimiting : Attribute
{
    
}