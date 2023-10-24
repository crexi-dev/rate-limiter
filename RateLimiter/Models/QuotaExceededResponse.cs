using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Models
{
    public class QuotaExceededResponse : IQuotaResponse
    {
        public string ContentType { get; set; } = "text/plain";

        public string Content { get; set; }

        public int? StatusCode { get; set; } = 429;

        public string RetryAfter { get; set; }

        public string Message { get; set; }

        public bool Passed { get; set; }
    }
}
