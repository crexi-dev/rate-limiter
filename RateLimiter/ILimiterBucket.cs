using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
    public interface ILimiterBucket
    {
        int Counter { get; }
        DateTime CreatedAt { get; }
        DateTime UpdatedAt { get; }
        int RefreshRate { get; }
        public void ProcessRequest();
        public void UpdateTokens();
    }
}
