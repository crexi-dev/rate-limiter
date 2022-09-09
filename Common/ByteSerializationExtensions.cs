using System.Text;
using System.Text.Json;

namespace Common
{
    public static class ByteSerializationExtensions
    {
        public static byte[] ToByteArray(this object objectToSerialize)
        {
            if (objectToSerialize is null)
            {
                return null;
            }

            return Encoding.Default.GetBytes(JsonSerializer.Serialize(objectToSerialize));
        }

        public static T FromByteArray<T>(this byte[] arrayToDeserialize) 
            where T : class
        {
            if (arrayToDeserialize is null)
            {
                return default(T);
            }

            return JsonSerializer.Deserialize<T>(Encoding.Default.GetString(arrayToDeserialize));
        }
    }
}