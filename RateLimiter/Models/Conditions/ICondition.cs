namespace RateLimiter.Models.Conditions
{
    public interface ICondition
    {
        bool IsMatch(IContext context);
    }
}
