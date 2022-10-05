using RateLimiter.DAL.Entities.Interfaces;

namespace RateLimiter.DAL.Entities;

public class History : Entity
{
    public Guid TokenId { get; set; }
    public virtual Token Token { get; set; }
    public Guid SetId { get; set; }
    public virtual Set? Set { get; set; }
    public DateTime LastCallInUtc { get; set; }
    public int? Count { get; set; }
}