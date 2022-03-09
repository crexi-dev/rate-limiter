using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using RateLimiter.Models.Domain.Comparers;

namespace RateLimiter.Models.Domain.Converters
{
    /// <summary>
    /// Custom converter for JSON serialization (marshalling)
    /// </summary>
    public class SortedSetJsonConverter : JsonConverter<SortedSet<Request>>
    {
        /// <summary>
        /// Reads and converts the JSON to type Request.
        /// </summary>
        public override SortedSet<Request> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
                throw new JsonException();

            reader.Read();

            var requests = new SortedSet<Request>(new TimestampComparer());

            while (reader.TokenType != JsonTokenType.EndArray)
            {
                requests.Add(JsonSerializer.Deserialize<Request>(ref reader, options)!);

                reader.Read();
            }

            return requests;
        }

        /// <summary>
        /// Writes a specified value as JSON.
        /// </summary>
        public override void Write(Utf8JsonWriter writer, SortedSet<Request> requests, JsonSerializerOptions options) =>
            JsonSerializer.Serialize(writer, requests, options);
    }
}
