using RateLimiter.Domain.ApiLimiter;
using RateLimiter.Infrastructure;

namespace RateLimiter.Api.Queries
{
    public class SearchQuery
    {
        public const string RESOURCE = "rs1";
        private IApiLimiter _apiLimiter;
        private IInMemoryRulesRepository _inMemoryRulesRepository;

        public SearchQuery(IApiLimiter apiLimiter, IInMemoryRulesRepository inMemoryRulesRepository)
        {
            _apiLimiter = apiLimiter;
            _inMemoryRulesRepository = inMemoryRulesRepository;
        }

        public bool Execute(string token)
        {
            bool verify = _apiLimiter.Verify(RESOURCE, token);
            return verify;
        }
    }
}
