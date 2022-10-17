using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RateLimiter.DataModel;

namespace RateLimiter.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RateLimiterController : ControllerBase
    {
        private readonly ILogger<RateLimiterController> _logger;
        private IRequestLimitChecker _limitChecker;

        public RateLimiterController(ILogger<RateLimiterController> logger, IRequestLimitChecker limitChecker)
        {
            _logger = logger;
            _limitChecker = limitChecker;
        }

        [HttpPost]
        public bool SubmitRequest(int clientId, int resourceId)
        {
            RequestData requestData = new RequestData();
            ClientRequest clientRequest = new ClientRequest() { ClientId = clientId, ResourceId = resourceId };
            requestData.ClientRequest = clientRequest;

            return _limitChecker.CanProcessRequest(requestData);
        }
    }
}
