namespace RateLimiter.Domain.Entities
{
    /// <summary>
    /// Resource A
    /// </summary>
    public class ResourceA : ResourceBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ResourceA()
        {
            ResourceName = "ResourceA";
        }

        /// <summary>
        /// An arbitrary property specific to ResourceA
        /// </summary>
        public string ResourceAProperty1 { get; set; }
    }
}
