using System.Collections.Generic;
using System.Threading.Tasks;

namespace RateLimiter.Interfaces.DataAccess
{
    /// <summary>
    /// Defines a repository interface for persisted data used by rate-limiting rules
    /// </summary>
    public interface IRateLimitRepository
    {
        /// <summary>
        /// Updates values for a given key
        /// </summary>
        /// <param name="key">
        /// A key that uniquely defines the data being stored. The key should
        /// be unique to the combination of endpoint, user, and verbs
        /// </param>
        /// <param name="values">A dictionary of values to store</param>
        /// <returns>An awaitable</returns>
        Task Update(string key, IDictionary<string, object> values);

        /// <summary>
        /// Retrieves values for a given key if it exists
        /// </summary>
        /// <param name="key">The unique key of previously stored values</param>
        /// <returns>The dictionary of value associated with the key supplied, if any exist</returns>
        Task<IDictionary<string, object>?> Retrieve(string key);
    }
}
