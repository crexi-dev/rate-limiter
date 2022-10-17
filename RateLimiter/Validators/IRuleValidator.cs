using RateLimiter.DataModel;

namespace RateLimiter.Validators
{
    public interface IRuleValidator
    {
        int RuleId { get; }
        bool Validate(RequestData requestData);
    }
}
