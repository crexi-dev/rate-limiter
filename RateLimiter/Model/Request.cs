using System;
namespace RateLimiter.Model
{
    /// <summary>
    /// Request to the API for access
    /// </summary>
    public class Request
    {
        #region Constructor
        public Request(AccessToken token)
        {
            Token = token;
            Creation = DateTime.Now;
        }
        #endregion
        #region Properties
        public AccessToken Token { get; set; }
        private DateTime Creation { get; set; }
        #endregion
    }
}

