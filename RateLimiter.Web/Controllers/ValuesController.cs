using RateLimiter.Data;
using RateLimiter.Filters;
using RateLimiter.Services;
using System;
using System.Linq;
using System.Web.Http;

namespace RateLimiter.Web.Controllers
{
    public class ValuesController : BaseApiController
    {
        public ValuesController(IAccessService accessService) : base(accessService)
        {
        }

        [HttpGet]
        [RateLimiterRequestRate(60, 4)]
        // GET api/values
        public IHttpActionResult GetLimitedByRequestRate()
        {
            int account = int.Parse(Request.Headers.GetValues("account").First() ?? "0");
            string token = Request.Headers.GetValues("token").First();

            AccessService.Add(new Access("127.0.0.1", account, token, DateTime.Now));

            return Ok("some content");
        }

        [HttpGet]
        [RateLimiterRequestRate(60, 4)]
        // GET api/values
        public IHttpActionResult GetLimitedPerTokenRegion()
        {
            int account = int.Parse(Request.Headers.GetValues("account").First() ?? "0");
            string token = Request.Headers.GetValues("token").First();

            AccessService.Add(new Access("127.0.0.1", account, token, DateTime.Now));

            return Ok("some content");
        }

        [HttpGet]
        [RateLimiterBlockIpRange("127.0.0.0", "127.0.0.99")]
        // GET api/values
        public IHttpActionResult GetFromBlockedEndpoint()
        {
            int account = int.Parse(Request.Headers.GetValues("account").First() ?? "0");
            string token = Request.Headers.GetValues("token").First();

            AccessService.Add(new Access("127.0.0.1", account, token, DateTime.Now));

            return Ok("some content");
        }

        [HttpGet]
        [RateLimiterPerTimeSpan(30)]
        // GET api/values
        public IHttpActionResult GetLimitedByTimeSpan()
        {
            int account = int.Parse(Request.Headers.GetValues("account").First() ?? "0");
            string token = Request.Headers.GetValues("token").First();

            AccessService.Add(new Access("127.0.0.1", account, token, DateTime.Now));

            return Ok("some content");
        }

        [HttpGet]
        [Route("api/values/no-limit")]
        public IHttpActionResult GetFromNoLimitEndpoint()
        {
            //dummy values
            int account = int.Parse(Request.Headers.GetValues("account").First() ?? "0");
            string token = Request.Headers.GetValues("token").First();

            AccessService.Add(new Access("127.0.0.1", account, token, DateTime.Now));
            return Ok("some unlimited content");
        }
    }
}
