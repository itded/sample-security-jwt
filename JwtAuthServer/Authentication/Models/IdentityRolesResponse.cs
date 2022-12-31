namespace JwtAuthServer.Authentication.Models
{
    public class IdentityRolesResponse: ResponseBase
    {
        public IdentityRolesResponse()
        {
        }

        public IdentityRolesResponse(params ResponseError[] errors) : base(errors)
        {
        }

        public string[] AddedRoleNames { get; set; }

        public string[] ErrorRoleNames { get; set; }

        public string[] ExistingRoleNames { get; set; }
    }
}
