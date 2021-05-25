namespace JwtAuthServer.Authentication.Models
{
    public class UserRegisterResponse : ResponseBase
    {
        public UserRegisterResponse() : base()
        {
        }

        public UserRegisterResponse(params ResponseError[] errors) : base(errors)
        {
        }
    }
}
