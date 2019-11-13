namespace RateLimiter
{
    using System;
    using System.Threading.Tasks;

    public class APIResourceController : IDisposable
    {
        private Session Session { get; }
        private string SessionStatus;
        /// <summary>
        /// Constructor to create in-memory session
        /// </summary>
        /// <param name="accessToken"></param>
        public APIResourceController(string accessToken)
        {
            Session = new Session(accessToken);
        }

        /// <summary>
        /// API resource to GetUsers
        /// </summary>
        /// <returns>users data or session message</returns>
        public async Task<string> GetUsers()
        {
            SessionStatus = Session.IsValidSession();
            if (SessionStatus == SessionMessage.ValidSession)
            {
                return await Task.FromResult("Data From GetUsers");
            }
            else
            {
                return await Task.FromResult(SessionStatus);
            }
        }

        /// <summary>
        /// API resource to GetUsersNames
        /// </summary>
        /// <returns>user names data or session message</returns>
        public async Task<string> GetUsersNames()
        {
            SessionStatus = Session.IsValidSession();
            if (SessionStatus == SessionMessage.ValidSession)
            {
                return await Task.FromResult("Data From GetUsersNames");
            }
            else
            {
                return await Task.FromResult(SessionStatus);
            }
        }

        /// <summary>
        /// Dispose resources used to free up memory 
        /// </summary>
        public void Dispose()
        {
            Session.Dispose(); 
        }
    }
}