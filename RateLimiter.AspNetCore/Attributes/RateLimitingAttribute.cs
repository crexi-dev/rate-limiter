namespace RateLimiter.AspNetCore.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class RateLimitingAttribute : Attribute
{
    public TimeSpan Interval { get; }
    public int Limit { get; }
    
    public RateLimitingAttribute(string intervalString, int limit = 1)
    {
        //TimeSpan can not be used as a parameter in attributes. TODO: Add a substitute of TimeSpan with 'Value' and 'Measure' (Seconds/Minutes/and Other)
        if(!TimeSpan.TryParse(intervalString, out var interval))
        {
            throw new ArgumentException($"The 'Interval' string has incorrect format: {intervalString}",
                nameof(intervalString));
        }
        
        Interval = interval;
        Limit = limit;
    }

    public List<RateLimiterParameter>? Parameters { get; set; }
}