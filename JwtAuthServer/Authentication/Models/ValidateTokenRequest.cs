namespace JwtAuthServer.Authentication.Models
{
    public class ValidateTokenRequest
    {
        public string UserName { get; set; }

        public string Token { get; set; }

        public string RefreshToken { get; set; }
    }
}
