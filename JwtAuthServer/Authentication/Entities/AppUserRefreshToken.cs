using System;
using Microsoft.AspNetCore.Identity;

namespace JwtAuthServer.Authentication.Entities
{
    public class AppUserRefreshToken : IdentityUserToken<long>
    {
        public DateTime Expires { get; set; }

        public DateTime Created { get; set; }

        public DateTime? Revoked { get; set; }
    }
}
