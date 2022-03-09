using System;
using System.Text;
using System.Text.Json;
using RateLimiter.Common.Utilities.Enums;

namespace RateLimiter.Common.Utilities
{
    /// <summary>
    ///  A helper class that provides common methods and utilities
    /// </summary>
    public static class Helper
    {
        private static readonly Random _random;

        static Helper()
        {
            _random = new Random();
        }

        /// <summary>
        /// Converts a object value to a byte array.
        /// </summary>
        /// <param name="value"></param>
        public static byte[] ToByteArray(this object value) =>
            value == null ? null : Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value));

        /// <summary>
        /// Converts a byte array to a object value.
        /// </summary>
        /// <param name="bytes"></param>
        public static T FromByteArray<T>(this byte[] bytes) where T : class =>
            bytes == null ? default : JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(bytes));

        /// <summary>
        /// Returns a random consistency level
        /// </summary>
        public static ConsistencyLevel GenerateRandomConsistencyLevel() => (ConsistencyLevel)_random.Next(1, 3);

        /// <summary>
        /// A method that that calculates a simple request unit (RU) charge
        /// </summary>
        /// <param name="totalSize"></param>
        /// <param name="level"></param>
        public static double CalculateRequestUnits(double totalSize, ConsistencyLevel level) =>
            level == ConsistencyLevel.Eventual ? Math.Round(totalSize / 100, 0) : 2 * Math.Round(totalSize / 100, 0);
    }
}
