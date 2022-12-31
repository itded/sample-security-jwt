namespace JwtAuth.Common.Models
{
    public class RotateTokenRequest
    {
        public string UserName { get; set; }

        public string Token { get; set; }

        public string RefreshToken { get; set; }
    }
}