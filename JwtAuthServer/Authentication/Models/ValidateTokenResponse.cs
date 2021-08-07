namespace JwtAuthServer.Authentication.Models
{
    public class ValidateTokenResponse : ResponseBase
    {
        public ValidateTokenResponse()
        {
        }

        public ValidateTokenResponse(params ResponseError[] errors) : base(errors)
        {
        }
    }
}
