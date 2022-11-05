namespace RateLimiter.Repository
{
    public interface IQueryRepository<T> where T : class
    {
        T GetById(string id);
    }
}
