using System;

namespace RateLimiter.Core.Rules
{
    public class GeographicMuxRule : IRule
    {
        public GeographicMuxRule(
            string sourceIdentifier,
            int timespanBetweenRequestsMs,
            int requestsPerTimespanMaxRequestCount,
            int requestsPerTimespanSeconds)
        {
            if (string.IsNullOrEmpty(sourceIdentifier))
            {
                throw new ArgumentException($"{nameof(sourceIdentifier)} must be provided.");
            }

            SourceIdentifier = sourceIdentifier;
            RequestsPerTimespanMaxRequestCount = requestsPerTimespanMaxRequestCount;
            RequestsPerTimespanSeconds = requestsPerTimespanSeconds;
            TimespanBetweenRequestsMS = timespanBetweenRequestsMs;
        }

        private string SourceIdentifier { get; }

        private int RequestsPerTimespanMaxRequestCount { get; }

        private int RequestsPerTimespanSeconds { get; }

        private int TimespanBetweenRequestsMS { get; }

        private readonly string TokenErrorMessage = $"Auth token must begin with '{US}' or '{EU}'.";

        public static readonly string US = "US";

        public static readonly string EU = "EU";

        public bool AllowExecution(string authToken)
        {
            if (string.IsNullOrEmpty(authToken))
            {
                throw new ArgumentException("Auth token must be provided.");
            }

            if (authToken.StartsWith(US))
            {
                return new RequestsPerTimespanRule(SourceIdentifier, RequestsPerTimespanMaxRequestCount, RequestsPerTimespanSeconds).AllowExecution(authToken);
            }
            else if (authToken.StartsWith(EU))
            {
                return new TimespanBetweenRequestsRule(SourceIdentifier, TimespanBetweenRequestsMS).AllowExecution(authToken);
            }

            throw new ArgumentException(TokenErrorMessage);
        }

        public string GetNotAllowedReason(string authToken)
        {
            if (string.IsNullOrEmpty(authToken))
            {
                throw new ArgumentException("Auth token must be provided.");
            }

            if (authToken.StartsWith(US))
            {
                return new RequestsPerTimespanRule(SourceIdentifier, RequestsPerTimespanMaxRequestCount, RequestsPerTimespanSeconds)
                    .GetNotAllowedReason(authToken);
            }
            else if (authToken.StartsWith(EU))
            {
                return new TimespanBetweenRequestsRule(SourceIdentifier, TimespanBetweenRequestsMS).GetNotAllowedReason(authToken);
            }

            throw new ArgumentException(TokenErrorMessage);
        }
    }
}