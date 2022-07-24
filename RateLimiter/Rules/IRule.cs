namespace RuleLimiterTask.Rules
{
    public interface IRule
    {
        bool IsValid(UserRequest request, ICacheService cache);
    }
}
