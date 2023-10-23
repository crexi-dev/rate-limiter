using RateLimiter.Contracts;
using System;
using System.ComponentModel.DataAnnotations;

namespace RateLimiter.Models
{
    public class RequestLimitRule : IRequestLimitRule
    {
        [Required]
        public byte RuleType { get; set; }
        [Required]
        public byte ResourceType { get; set; }
        [Required]
        public TimeSpan Time { get; set; }
        public int CountLimit { get; set; }
        public int? RegionId { get; set; }
    }
}
