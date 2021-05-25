namespace JwtAuthServer.Authentication.Models
{
    /// <summary>
    /// Represents a response error.
    /// </summary>
    public class ResponseError
    {
        public string Code { get; set; }

        public string Description { get; set; }
    }
}
