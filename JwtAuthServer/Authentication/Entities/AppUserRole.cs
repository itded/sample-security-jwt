using Microsoft.AspNetCore.Identity;

namespace JwtAuthServer.Authentication.Entities
{
    /// <summary>
    /// Extends the <see cref="IdentityUserRole{TKey}"/> class.
    /// </summary>
    public class AppUserRole : IdentityUserRole<long>
    {
    }
}
