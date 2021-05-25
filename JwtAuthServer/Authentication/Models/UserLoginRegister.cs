namespace JwtAuthServer.Authentication.Models
{
    public class UserLoginRequest
    {
        public string UserName { get; set; }

        public string Password { get; set; }
    }
}
