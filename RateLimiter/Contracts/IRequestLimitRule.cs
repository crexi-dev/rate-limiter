using System;
using System.ComponentModel.DataAnnotations;

namespace RateLimiter.Contracts
{
    public interface IRequestLimitRule
    {
        [Required]
        byte RuleType { get; set; }
        [Required]
        byte ResourceType { get; set; }
        [Required]
        TimeSpan Time { get; set; }

        int CountLimit { get; set; }

        int? RegionId { get; set; }     

    }
}
