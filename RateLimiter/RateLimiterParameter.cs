namespace RateLimiter;

public record RateLimiterParameter
{
    public string Name { get; }
    
    public string Value { get; }
    
    public RateLimiterParameter(string name, string value)
    {
        Name = name;
        Value = value;
    } 
}