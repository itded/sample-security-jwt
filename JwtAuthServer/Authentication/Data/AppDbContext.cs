﻿using JwtAuthServer.Authentication.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JwtAuthServer.Authentication.Data
{
    /// <summary>
    /// The application's identity DB context.
    /// </summary>
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, long, AppUserClaim, AppUserRole,
        AppUserLogin, AppRoleClaim, AppUserToken>
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            // nothing
        }
    }
}
