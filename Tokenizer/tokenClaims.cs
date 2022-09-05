namespace Tokenizer
{
    public class TokenClaims
    {
        public TokenClaims(string clientId, string region)
        {
            ClientId = clientId;
            Region = region;
        }

        public string ClientId { get; set; }
        public string Region { get; set; }
    }
}
