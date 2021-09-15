namespace JwtAuthServer.Authentication.Models
{
    public class AddUserToRolesRequest
    {
        public string UserName { get; set; }

        public string[] RoleNames { get; set; }
    }
}
