using RateLimiter.Model.Response;

namespace RateLimiter.Helper
{
    internal class HelperResponse
    {
        public static T GetSuccess<T>(string message = "Success") where T : ResponseGeneral, new()
        {
            return GetResponse<T>(message, EnumResponseCode.Success);
        }

        public static T GetException<T>(string message = "We encountered an unexpected error") where T : ResponseGeneral, new()
        {
            return GetResponse<T>(message, EnumResponseCode.Exception);
        }

        public static T GetFailure<T>(string message) where T : ResponseGeneral, new()
        {
            return GetResponse<T>(message, EnumResponseCode.Failed);
        }

        private static T GetResponse<T>(string message, EnumResponseCode code) where T : ResponseGeneral, new()
        {
            return new T
            {
                ResponseCode = (int)code,
                ResponseMessage = message
            };
        }
    }
}
