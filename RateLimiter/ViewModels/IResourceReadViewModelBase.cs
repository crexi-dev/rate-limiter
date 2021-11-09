using System;

namespace RateLimiter.ViewModels
{

    /// <summary>
    /// Base for resource view models
    /// </summary>
    public interface IResourceReadViewModelBase
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the date this resource was last updated.
        /// </summary>
        public DateTime Updated { get; set; }

        /// <summary>
        /// Gets or sets the date the resource was retrieved.
        /// </summary>
        public DateTime Retrieved { get; set; }
    }
}