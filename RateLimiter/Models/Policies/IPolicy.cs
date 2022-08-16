namespace RateLimiter.Models.Policies
{
    public interface IPolicy
    {
        PolicyResult Execute(СlientStatistics сlientStatistics, IContext context);
    }
}
