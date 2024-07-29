using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RateLimiter;

public class RuleSettingsList
{
    [JsonPropertyName("RateLimiterRules")]
    public List<RuleSettings>? RateLimiterRules { get; set; }
}

public class RuleSettings
{
    [JsonPropertyName("Name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("Enabled")]
    public bool Enabled { get; set; } = false;

    [JsonPropertyName("Options")]
    public List<Option>? Options { get; set; } = default;
}

public class Option
{
    [JsonPropertyName("Type")]
    public string? Type { get; set; }

    [JsonPropertyName("Region")]
    public string? Region { get; set; }

    [JsonPropertyName("MaxRequests")]
    public int? MaxRequests { get; set; }

    [JsonPropertyName("PeriodMsec")]
    public int? PeriodMsec { get; set; }

    [JsonPropertyName("CooldownMsec")]
    public int? CooldownMsec { get; set; }

    [JsonPropertyName("Tier")]
    public string? Tier { get; set; }
}