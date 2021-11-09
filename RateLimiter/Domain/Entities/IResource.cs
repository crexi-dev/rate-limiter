using System;

namespace RateLimiter.Domain.Entities
{
    /// <summary>
    /// Represents a (very abstract) resource. Implementations include other properties as appropriate.
    /// </summary>
    public interface IResource
    {
        /// <summary>
        /// Name.
        /// </summary>
        string ResourceName { get; set; }

        /// <summary>
        /// Unique identifier.
        /// </summary>
        int Id { get; set; }

        /// <summary>
        /// When this resource was last updated.
        /// </summary>
        DateTime Updated { get; set; }
    }
}