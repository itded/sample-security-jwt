using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace JwtAuthServer.Authentication.Entities
{
    /// <summary>
    /// Extends the <see cref="IdentityRole{TKey}"/> class.
    /// </summary>
    public class AppRole : IdentityRole<long>
    {
        private const int DescriptionMaxLength = 256;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="name">The role name.</param>
        public AppRole(string name) : base(name)
        {
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="name">The role name.</param>
        /// <param name="description">The role description.</param>
        public AppRole(string name, string description) : this(name)
        {
            Description = description;
        }

        /// <summary>
        /// Gets or sets the description for this role.
        /// </summary>
        [MaxLength(DescriptionMaxLength)]
        public string Description { get; set; }
    }
}
