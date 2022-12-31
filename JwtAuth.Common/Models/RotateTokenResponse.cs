namespace JwtAuth.Common.Models
{
    public class RotateTokenResponse
    {
        public string JwtToken { get; set; }

        public string RefreshToken { get; set; }
    }
}