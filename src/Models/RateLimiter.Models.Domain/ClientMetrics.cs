using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using RateLimiter.Models.Domain.Comparers;
using RateLimiter.Models.Domain.Converters;

namespace RateLimiter.Models.Domain
{
    public class ClientMetrics
    {
        public string AccessKey { get; set; }
        public int TotalRequests { get; set; }
        public long TotalSize { get; set; }
        public double TotalRequestUnits { get; set; }
        public DateTime? ExpiresAt { get; set; }

        [JsonConverter(typeof(SortedSetJsonConverter))]
        public SortedSet<Request> Requests { get; set; }

        public ClientMetrics(string accessKey, int totalRequests, long totalSize, double totalRequestUnits, DateTime? expiresAt = null)
        {
            AccessKey = accessKey;
            TotalRequests = totalRequests;
            TotalSize = totalSize;
            TotalRequestUnits = totalRequestUnits;
            ExpiresAt = expiresAt;
            Requests = new SortedSet<Request>(new TimestampComparer());
        }
    }
}
