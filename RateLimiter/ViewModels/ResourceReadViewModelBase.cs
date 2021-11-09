using System;

namespace RateLimiter.ViewModels
{
    /// <inheritdoc />
    public class ResourceReadViewModelBase : IResourceReadViewModelBase
    {
        /// <inheritdoc />
        public int Id { get; set; }

        /// <inheritdoc />
        public DateTime Updated { get; set; }

        /// <inheritdoc />
        public DateTime Retrieved { get; set; }
    }
}