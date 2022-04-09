namespace RateLimiter.DataStorageSimulator
{
    /// <summary>
    /// Simulates the List of the tokens that can be received with the requests
    /// Each Client has different token
    /// </summary>
    public enum Token
    {
        UnRatedClientToken = 0,
        ClientAToken = 1,
        ClientBToken = 2,
        ClientCToken = 3
    }
}
