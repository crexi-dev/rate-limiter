namespace RateLimiter.Repository
{
    public interface IRepository<T> : IQueryRepository<T> where T : class
    {
        T AddOrReplace(T entity);
    }
}
