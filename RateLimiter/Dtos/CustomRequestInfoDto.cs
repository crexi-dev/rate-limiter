using RateLimiter.Nugget.Dtos;

namespace RateLimiter.Dtos
{
    internal class CustomRequestInfoDto : BaseRequestInfoDto
    {
        public string DeviceId { get; set; }
    }
}
