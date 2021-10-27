using RateLimiter.Domain.ApiLimiter;
using RateLimiter.Infrastructure;

namespace RateLimiter.Api.Queries
{
    public class UpdateCommand
    {
        public const string RESOURCE = "up1";
        private IApiLimiter _apiLimiter;
        private IInMemoryRulesRepository _inMemoryRulesRepository;

        public UpdateCommand(IApiLimiter apiLimiter, IInMemoryRulesRepository inMemoryRulesRepository)
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
