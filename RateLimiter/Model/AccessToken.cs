using System;
namespace RateLimiter.Model
{

    /// <summary>
    /// Used to denote region for an AccessToken
    /// </summary>
    public enum Region
    {
        CN,
        EU,
        LA,
        US
    }
    /// <summary>
    /// Used to denote Priority level for an AccessToken
    /// </summary>
    public enum PriorityLevel
    {
        Basic,
        Paid,
        Premium,
        Blocked
    }
    /// <summary>
    /// Used to Authorize API access
    /// </summary>
    public class AccessToken
    {
        #region Constructor
        public AccessToken(string name, Region region, PriorityLevel level)
        {
            Name = name;
            Region = region;
            Level = level;
            TokenCreation = DateTime.Now;
            GenerateToken();
        }
        #endregion
        #region Properties
        public string Name { get; set; }
        public Region Region { get; set; }
        public PriorityLevel Level { get; set; }
        public string Token { get; set; }
        private DateTime TokenCreation { get; set; }
        #endregion
        #region Methods
        public TimeSpan TokenAge()
        {
            return DateTime.Now.Subtract(TokenCreation);
        }
        private void GenerateToken()
        {
            Token = Level.ToString() + "|" + Name + Region + TokenCreation.ToString("MMddyyyTH:mm:ss.fff");
        }
        #endregion
    }
}