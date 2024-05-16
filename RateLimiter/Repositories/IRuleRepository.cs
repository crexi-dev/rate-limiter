using RateLimiter;

public interface IRuleRepository
{
    IRuleCollection GetRules(Token token);
}