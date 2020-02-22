namespace RateLimiter.State
{
    public interface IRulePersist
    {
        bool Put<T>(string key, T value);

        (T val,bool fnd) Retrieve<T>(string key);
    }
}