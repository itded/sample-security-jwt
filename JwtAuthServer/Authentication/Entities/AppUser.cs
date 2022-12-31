using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace JwtAuthServer.Authentication.Entities
{
    /// <summary>
    /// Extends the <see cref="IdentityUser{TKey}"/> class.
    /// </summary>
    /// <remarks>
    /// PersonalData - https://docs.microsoft.com/en-us/aspnet/core/security/authentication/add-user-data?view=aspnetcore-5.0&tabs=visual-studio
    /// </remarks>
    public class AppUser : IdentityUser<long>
    {
        private const int NameMaxLength = 128;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="userName">The user name.</param>
        public AppUser(string userName) : base(userName)
        {
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="firstName">The user's first name.</param>
        /// <param name="lastName">The user's last name.</param>
        /// <param name="userName">The user name.</param>
        public AppUser(string firstName, string lastName, string userName): this(userName)
        {
            FirstName = firstName;
            LastName = lastName;
        }

        /// <summary>
        /// Gets or sets the fist name for this user.
        /// </summary>
        [PersonalData]
        [MaxLength(NameMaxLength)]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name for this user.
        /// </summary>
        [PersonalData]
        [MaxLength(NameMaxLength)]
        public string LastName { get; set; }
    }
}
