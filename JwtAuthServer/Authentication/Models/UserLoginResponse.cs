namespace JwtAuthServer.Authentication.Models
{
    public class UserLoginResponse : ResponseBase
    {
        public UserLoginResponse()
        {
        }

        public UserLoginResponse(params ResponseError[] errors) : base(errors)
        {
        }

        public UserInfo UserInfo { get; set; }

        public string JwtToken { get; set; }

        public string RefreshToken { get; set; }
    }
}
