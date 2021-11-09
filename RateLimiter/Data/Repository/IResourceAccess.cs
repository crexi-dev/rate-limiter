using System;

namespace RateLimiter.Data.Repository
{
    /// <summary>
    /// Interface IResourceAccess, stores views of a resource associated with a user
    /// </summary>
    public interface IResourceAccess
    {
        /// <summary>
        /// Gets or sets the name of the resource.
        /// </summary>
        string ResourceName { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        int Id { get; set; }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        int UserId { get; set; }

        /// <summary>
        /// Gets or sets the accessed.
        /// </summary>
        DateTime Accessed { get; set; }
    }
}