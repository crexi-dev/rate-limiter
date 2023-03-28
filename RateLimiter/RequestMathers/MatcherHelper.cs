namespace RateLimiter.RequestMathers;

internal static class MatcherHelper
{
    internal const string AllItems = "*";

    public static bool IsAllRequestMatch(string filter)
    {
        return AllItems.Equals(filter);
    }
}