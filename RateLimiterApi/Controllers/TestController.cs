using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;

namespace RateLimiterApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet("GetAll")]
        public string TestGetAll()
        {
            return "Ok";
        }

        [HttpGet("Get")]
        public string TestGet()
        {
            return "Ok";
        }

        [HttpPost("Post")]
        public string TestPost()
        {
            return "Ok";
        }
    }
}
