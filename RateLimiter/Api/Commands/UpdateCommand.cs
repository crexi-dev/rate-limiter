using RateLimiter.Domain.ApiLimiter;
using RateLimiter.Domain.Resource;
using RateLimiter.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Api.Queries
{
    public class UpdateCommand
    {
        public const string SUCCESS = "Success";
        public const string FAIL = "Fail";
        public const string RESOURCE = "up1";
        private IApiLimiter _apiLimiter;
        private IInMemoryRulesRepository _inMemoryRulesRepository;

        public UpdateCommand(IApiLimiter apiLimiter, IInMemoryRulesRepository inMemoryRulesRepository)
        {
            _apiLimiter = apiLimiter;
            _inMemoryRulesRepository = inMemoryRulesRepository;
        }

        public string Execute(string token)
        {
            bool verify = _apiLimiter.Verify(RESOURCE, token);
            return verify ? SUCCESS : FAIL;
        }
    }
}
