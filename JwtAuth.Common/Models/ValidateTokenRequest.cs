namespace JwtAuth.Common.Models
{
    public class ValidateTokenRequest
    {
        public string UserName { get; set; }

        public string Token { get; set; }
    }
}