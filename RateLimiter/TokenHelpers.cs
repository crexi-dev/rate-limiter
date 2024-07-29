namespace RateLimiter;

public static class TokenHelpers
{
    public static bool IsUsBased(this string token)
    {
        return token.Contains("US");
    }
}