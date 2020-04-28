using RateLimiter.Services;
using System.Web.Http;

namespace RateLimiter.Web.Controllers
{
    public abstract class BaseApiController : ApiController
    {
        public IAccessService AccessService;

        public BaseApiController(IAccessService accessService) {
            AccessService = accessService;
        }

    }
}