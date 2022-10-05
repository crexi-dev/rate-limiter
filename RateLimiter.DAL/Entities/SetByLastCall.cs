using RateLimiter.DAL.Entities.Interfaces;

namespace RateLimiter.DAL.Entities;

public class SetByLastCall : Set
{
    public double SuspenseTimeInMilliseconds { get; set; }
}