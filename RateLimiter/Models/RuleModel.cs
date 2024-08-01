using RateLimiter.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RateLimiter.Models
{
    public class RuleModel
    {
        [JsonPropertyName("Type")]
        public RuleType? Type {  get; set; }
        [JsonPropertyName("MaxRequests")]
        public int? MaxRequests { get; set; }
        [JsonPropertyName("Window")]
        public TimeSpan? Window { get; set; }
        [JsonPropertyName("Locations")]
        public List<string>? Locations { get; set; } 

        public RuleModel(RuleType? type, int? maxRequests, TimeSpan? window, List<string>? locations)
        {
            Type = type;
            MaxRequests = maxRequests;
            Window = window;
            Locations = locations ?? new();
        }

        public RuleModel()
        {
        }

    }
}
