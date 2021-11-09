using System;

namespace RateLimiter.Data.Repository
{
    /// <inheritdoc />
    public class ResourceAccess : IResourceAccess
    {
        /// <inheritdoc />
        public string ResourceName { get; set; }

        /// <inheritdoc />
        public int Id { get; set; }

        /// <inheritdoc />
        public int UserId { get; set; }

        /// <inheritdoc />
        public DateTime Accessed { get; set; }
    }
}