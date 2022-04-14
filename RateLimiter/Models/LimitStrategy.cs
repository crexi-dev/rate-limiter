using System.ComponentModel;

namespace RateLimiter.Models
{
    public enum LimitStrategy
    {
        [Description("None")]
        None = 0,
        [Description("Header")]
        Header = 1,
        [Description("Endpoint")]
        Endpoint = 2,
        [Description("IP Address")]
        IP = 3
    }
}
