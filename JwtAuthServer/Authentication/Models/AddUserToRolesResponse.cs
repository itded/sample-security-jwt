namespace JwtAuthServer.Authentication.Models
{
    public class AddUserToRolesResponse: ResponseBase
    {
        public AddUserToRolesResponse()
        {
        }

        public AddUserToRolesResponse(params ResponseError[] errors) : base(errors)
        {
        }

        public string[] AddedRoleNames { get; set; }

        public string[] ErrorRoleNames { get; set; }
    }
}
