using Microsoft.AspNetCore.Identity;

namespace JwtAuthServer.Authentication.Entities
{
    /// <summary>
    /// Extends the <see cref="IdentityUserToken{TKey}"/> class.
    /// </summary>
    public class AppUserToken : IdentityUserToken<long>
    {
    }
}
