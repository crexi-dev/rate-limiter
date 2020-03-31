namespace RateLimiter.Core
{
    public interface IRule
    {
        bool AllowExecution(string authToken);

        string GetNotAllowedReason(string authToken);
    }
}
