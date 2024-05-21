using RateLimiter.Nugget.Dtos;

namespace RateLimiter.Nugget.DataStore
{
    public interface ICacheServiceBase<T> where T : BaseRequestInfoDto
    {
        //TODO Implement cache service
        public void AddRequestInfo(T requestInfo);

        public T GetRequestInfo(string token);
    }

    public interface ICacheServiceBase
    {
        //TODO Implement cache service
        public void AddRequestInfo(BaseRequestInfoDto requestInfo);

        public BaseRequestInfoDto GetRequestInfo(string token);
    }
}
