
namespace RateLimiter.Validators
{
    public interface IRuleValidatorFactory
    {
        IRuleValidator GetValidator(int ruleId);
    }
}
