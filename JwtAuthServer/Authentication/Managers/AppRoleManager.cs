using System.Collections.Generic;
using JwtAuthServer.Authentication.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace JwtAuthServer.Authentication.Managers
{
    public class AppRoleManager : RoleManager<AppRole>
    {
        public AppRoleManager(IRoleStore<AppRole> store, IEnumerable<IRoleValidator<AppRole>> roleValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, ILogger<RoleManager<AppRole>> logger)
            : base(store, roleValidators, keyNormalizer, errors, logger)
        {
            // nothing
        }
    }
}
