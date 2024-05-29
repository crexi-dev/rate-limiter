namespace RateLimiter.Models
{
    public class RuleCheckResult
    {
        public bool IsAllowed { get; }
        public string? ErrorMessage { get; }
        public string? ErrorCode { get; }

        public RuleCheckResult(bool isAllowed, string? errorMessage = null, string? errorCode = null)
        {
            IsAllowed = isAllowed;
            ErrorMessage = errorMessage;
            ErrorCode = errorCode;
        }
    }
}
