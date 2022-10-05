using RateLimiter.DAL.Entities.Interfaces;

namespace RateLimiter.DAL.Entities;

public class Token : Entity
{
    public Set? Set { get; set; }
    public Guid SetId { get; set; }

    public virtual History? History { get; set; }
    public Guid HistoryId { get; set; }
}