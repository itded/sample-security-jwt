using Microsoft.AspNetCore.Identity;

namespace JwtAuthServer.Authentication.Entities
{
    /// <summary>
    /// Extends the <see cref="IdentityRole{TKey}"/> class.
    /// </summary>
    public class AppRole : IdentityRole<long>
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        public AppRole()
        {
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="name">The role name.</param>
        public AppRole(string name) : this()
        {
            base.Name = name;
        }
    }
}
