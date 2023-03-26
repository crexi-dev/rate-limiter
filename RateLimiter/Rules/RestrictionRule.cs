namespace RateLimiter.Rules;

/// <summary>
/// Refuse all request from a token
/// </summary>
public class RestrictionRule : IRule
{
    public string? ErrorMessage { get; }
    public bool CheckAndUpdate(UserRegInfo userRegInfo)
    {
        return false;
    }
}