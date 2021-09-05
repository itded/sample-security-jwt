using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JwtAuthServer.Authentication.Entities
{
    public class AppRevokedUserRefreshToken
    {
        /// <summary>
        /// Gets or sets the entity identifier.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual long Id { get; set; }

        public long UserId { get; set; }

        public string LoginProvider { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public DateTime Expires { get; set; }

        public DateTime Created { get; set; }
    }
}
