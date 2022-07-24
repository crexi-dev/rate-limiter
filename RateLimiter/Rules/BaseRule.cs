namespace RuleLimiterTask.Rules
{
    public abstract class BaseRule : IRule
    {
        public abstract bool IsValid(UserRequest request, ICacheService cache);

        protected string GenerateKey(int userId) => $"{GetType().Name}_{userId}";
    }
}
