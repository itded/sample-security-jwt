namespace JwtAuthServer.Authentication.Models
{
    public class VerifyTokenRequest
    {
        public string Token { get; set; }

        public string RefreshToken { get; set; }
    }
}
