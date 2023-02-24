namespace RateLimiter.Configuration
{
    public class RateLimiterOptions
    {
        public IResourceOptions For(string resourceName)
        {
            throw new System.NotImplementedException();
        }
    }

    public interface IResourceOptions
    {
        IResourceOptions AddRule<TRule>(TRule rule);
    }
}