namespace JwtAuth.Common.Models
{
    public class UserLoginResponse
    {
        public string UserName { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string JwtToken { get; set; }

        public string RefreshToken { get; set; }
    }
}