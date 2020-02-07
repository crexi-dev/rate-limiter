using RateLimiter.Api.Attributes;
using System.Collections.Generic;
using System.Web.Http;

namespace RateLimiter.Api.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        [ValidateRateLimit(applyTimeRules: false, applyQuantityRules: true)]
        public IEnumerable<string> Get()
        {
            return new[] { "value1", "value2" };
        }

        // GET api/values/5
        [ValidateRateLimit(applyTimeRules: true, applyQuantityRules: false)]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [ValidateRateLimit(applyTimeRules: true, applyQuantityRules: true)]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
