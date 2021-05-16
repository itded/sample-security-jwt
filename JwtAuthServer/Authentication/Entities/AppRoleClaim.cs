using Microsoft.AspNetCore.Identity;

namespace JwtAuthServer.Authentication.Entities
{
    /// <summary>
    /// Extends the <see cref="IdentityRoleClaim{TKey}"/> class.
    /// </summary>
    public class AppRoleClaim : IdentityRoleClaim<long>
    {
    }
}
