namespace RateLimiter.Domain.Entities
{
    /// <summary>
    /// Resource B
    /// </summary>
    public class ResourceB : ResourceBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ResourceB()
        {
            ResourceName = "ResourceB";
        }

        /// <summary>
        /// An arbitrary property specific to ResourceA
        /// </summary>
        public string ResourceBProperty1 { get; set; }

        /// <summary>
        /// An differetnt arbitrary property specific to ResourceA
        /// </summary>
        public string ResourceBProperty2 { get; set; }
    }
}