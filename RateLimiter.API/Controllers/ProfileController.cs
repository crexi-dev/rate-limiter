using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RateLimiter.API.ActionFilters;

namespace RateLimiter.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(ILogger<ProfileController> logger)
        {
            _logger = logger;
        }

        [TimespanBetweenRequests("profile:name", 500)]
        [HttpGet]
        [Route("name")]
        public async Task<string> GetName(string authToken)
        {
            return "Anshul";
        }

        [RequestsPerTimespan("profile:age", 5, 30)]
        [HttpGet]
        [Route("age")]
        public async Task<int> GetAge(string authToken)
        {
            return 32;
        }

        [GeographicMux("profile:city", 2000, 4, 10)]
        [HttpGet]
        [Route("city")]
        public async Task<string> GetCity(string authToken)
        {
            return "Los Angeles, CA";
        }
    }
}
