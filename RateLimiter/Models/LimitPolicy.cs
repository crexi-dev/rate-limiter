using System.ComponentModel;

namespace RateLimiter.Models
{
    public enum LimitPolicy
    {
        [Description("None")]
        None = 0,
        [Description("RequestsPerTimeSpan")]
        RequestsPerTimeSpan = 1,
        [Description("TimeSpanSinceLastRequest")]
        TimeSpanSinceLastRequest = 2
    }
}
