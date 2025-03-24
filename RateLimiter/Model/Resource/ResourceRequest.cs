namespace RateLimiter.Model.Resource
{
    public class ResourceRequest
    {
        public string Token { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public EnumResource Resource { get; set; } = EnumResource.NotSet;
    }
}
