using Microsoft.AspNetCore.Identity;

namespace JwtAuthServer.Authentication.Entities
{
    /// <summary>
    /// Extends the <see cref="IdentityUserLogin{TKey}"/> class.
    /// </summary>
    public class AppUserLogin : IdentityUserLogin<long>
    {
    }
}
