namespace RateLimiter.Rules
{
    public interface IRuleNode
    {
        void Accept(RulesVisitor v);
    }
}