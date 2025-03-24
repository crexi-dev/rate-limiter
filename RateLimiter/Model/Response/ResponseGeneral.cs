namespace RateLimiter.Model.Response
{
    public class ResponseGeneral
    {
        public int ResponseCode { get; set; } = (int)EnumResponseCode.Success;
        public string ResponseMessage { get; set; } = "Success";
        public bool Success => ResponseCode == (int)EnumResponseCode.Success;
    }
}
