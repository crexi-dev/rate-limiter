using RateLimiter.DAL.Entities.Interfaces;

namespace RateLimiter.DAL.Entities;

public class SetByCountPerTime : Set
{
    public int Count { get; set; }
    public double PerTimeInMilliseconds { get; set; }
}