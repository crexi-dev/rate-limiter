using System;

namespace RateLimiter.Domain.Entities
{
    ///<inheritdoc />
    public abstract class ResourceBase : IResource
    {
        ///<inheritdoc />
        public string ResourceName { get; set; }

        ///<inheritdoc />
        public int Id { get; set; }

        ///<inheritdoc />
        public DateTime Updated { get; set; }
    }
}