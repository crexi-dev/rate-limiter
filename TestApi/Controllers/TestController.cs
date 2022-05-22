using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TestApi.Controllers
{
    [ApiController]
    [Route("api/[Controller]/[action]")]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public string TestEndpoint2()
        {
            return "test endpoint Result";
        }
    }
}
