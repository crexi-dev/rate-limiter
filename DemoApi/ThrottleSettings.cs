namespace DemoApi
{
    public class ThrottleSettings
    {
        public (bool allow, string region) ThrottleMaxRequestPerTime { get; set; }
        public (bool allow, string region) ThrottlePassedSinceLastRequest { get; set; }
    }
}
