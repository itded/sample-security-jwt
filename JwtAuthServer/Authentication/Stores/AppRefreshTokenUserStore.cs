using System;
using System.Threading.Tasks;
using JwtAuthServer.Authentication.Data;
using JwtAuthServer.Authentication.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace JwtAuthServer.Authentication.Stores
{
    public class AppRefreshTokenUserStore : UserStore<AppUser, AppRole, AppDbContext, long>
    {
        public AppRefreshTokenUserStore(AppDbContext context, IdentityErrorDescriber describer = null) : base(context, describer)
        {
        }

        protected override IdentityUserToken<long> CreateUserToken(AppUser user, string loginProvider, string name, string value)
        {
            return new AppUserRefreshToken
            {
                UserId = user.Id,
                LoginProvider = loginProvider,
                Name = name,
                Value = value,
                Expires = DateTime.UtcNow.AddMinutes(5),
                Created = DateTime.UtcNow
            };
        }

        protected override Task AddUserTokenAsync(IdentityUserToken<long> token)
        {
            var refreshToken = token as AppUserRefreshToken;
            Context.RefreshTokens.Add(refreshToken ?? throw new InvalidOperationException());
            return Task.CompletedTask;
        }
    }
}
