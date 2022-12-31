namespace JwtAuth.Common.Models
{
    public class ValidateTokenResponse
    {
        public bool Succeeded { get; set; }

        public string UserName { get; set; }

        public string[] Roles { get; set; }
    }
}