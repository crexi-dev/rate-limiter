namespace ResourceApi.Models
{
    public class User
    {
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
    }
}
