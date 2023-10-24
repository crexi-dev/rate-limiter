using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Models
{
    public interface IQuotaResponse
    {
        bool Passed { get; set; }

        string ContentType { get; set; }

        string Content { get; set; }

        int? StatusCode { get; set; }

        string RetryAfter { get; set; }

        string Message { get; set; }
    }
}
