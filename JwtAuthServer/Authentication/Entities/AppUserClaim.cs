using Microsoft.AspNetCore.Identity;

namespace JwtAuthServer.Authentication.Entities
{
    /// <summary>
    /// Extends the <see cref="IdentityUserClaim{TKey}"/> class.
    /// </summary>
    public class AppUserClaim : IdentityUserClaim<long>
    {
    }
}
