namespace JwtAuthServer.Authentication.Models
{
    public class IdentityRolesRequest
    {
        public IdentityRole[] IdentityRoles { get; set; }

        public bool ContinueOnError { get; set; }

        public class IdentityRole
        {
            public string RoleName { get; set; }

            public string Description { get; set; }
        }
    }
}
