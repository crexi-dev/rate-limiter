namespace RuleLimiterTask
{
    public class UserRequest
    {
        public RequestState State { get; set; }
        public Token Token { get; }
        public DateTime RequestTime { get; private set; }

        public UserRequest(Region region, int userId)
        {
            Token = new Token(region, userId);
        }

        public void RequestAccess(Resource resource, ICacheService cache)
        {
            RequestTime = DateTime.Now;

            var accessGranted = resource.CheckAccess(this, cache);

            State = accessGranted ? RequestState.Success : RequestState.AccessDenied;
        }
    }
}
