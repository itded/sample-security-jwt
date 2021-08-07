namespace JwtAuthServer.Authentication.Models
{
    public class RotateTokenResponse: ResponseBase
    {
        public RotateTokenResponse()
        {
        }

        public RotateTokenResponse(params ResponseError[] errors) : base(errors)
        {
        }

        public string JwtToken { get; set; }

        public string RefreshToken { get; set; }
    }
}
