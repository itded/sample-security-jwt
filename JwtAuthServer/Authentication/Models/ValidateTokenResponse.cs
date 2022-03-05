namespace JwtAuthServer.Authentication.Models
{
    public class ValidateTokenResponse : ResponseBase
    {
        public string UserName { get; set; }

        public string[] Roles { get; set; }

        public ValidateTokenResponse()
        {
        }

        public ValidateTokenResponse(params ResponseError[] errors) : base(errors)
        {
        }
    }
}
