using RateLimiter.Nugget.Dtos;

namespace RateLimiter.Nugget.DataStore
{
    public class CacheServiceBase<T> : ICacheServiceBase<T> where T : BaseRequestInfoDto
    {
        public List<T> ListData = new List<T>();
        //TODO Implement cache service
        
        public void AddRequestInfo(T requestInfo)
        {
            // Add request info to cache
            ListData.Add(requestInfo);
        }

        public T GetRequestInfo(string token)
        {
            // Get request info from cache
            return (T)Convert.ChangeType("", typeof(T));
        }
    }

    internal class CacheServiceBase : CacheServiceBase<BaseRequestInfoDto>, ICacheServiceBase
    {

    }
}
