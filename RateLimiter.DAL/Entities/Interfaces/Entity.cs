using System.ComponentModel.DataAnnotations;

namespace RateLimiter.DAL.Entities.Interfaces;

public abstract class Entity
{
    [Key] public Guid Id { get; set; }
}