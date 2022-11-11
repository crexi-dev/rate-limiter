namespace RateLimiter;

public interface IRateLimiterStorage
{
    public void AddOrUpdate(string key, Func<string, ResourceEntry> addValueFactory,
        Func<string, ResourceEntry, ResourceEntry> updateValueFactory);
}