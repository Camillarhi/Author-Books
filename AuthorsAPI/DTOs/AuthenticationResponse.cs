namespace AuthorsAPI.DTOs
{
    public class AuthenticationResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
    }
}
