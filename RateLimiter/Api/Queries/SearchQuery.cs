using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Api.Queries
{
    public class SearchQuery
    {
        public const string SUCCESS = "Success";
        public const string FAIL = "Fail";
        public const string RESOURCE = "rs1";

        public string Execute(string token)
        {

            return SUCCESS;
        }
    }
}
