using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JwtAuthServer.Authentication.Data;
using JwtAuthServer.Authentication.Entities;
using JwtAuthServer.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace JwtAuthServer.Authentication.Stores
{
    public class AppRefreshTokenUserStore : UserStore<AppUser, AppRole, AppDbContext, long, AppUserClaim, AppUserRole, AppUserLogin, AppUserRefreshToken, AppRoleClaim>
    {
        private readonly IConfiguration _config;
        private const int DefaultRefreshTokenTtlInMinutes = 5;

        public AppRefreshTokenUserStore(AppDbContext context, IConfiguration config, IdentityErrorDescriber describer = null) : base(context, describer)
        {
            _config = config;
        }

        protected override AppUserRefreshToken CreateUserToken(AppUser user, string loginProvider, string name, string value)
        {
            var settings = _config.GetSection(JwtSettings.Position).Get<JwtSettings>();
            var refreshTokenTtl = settings.RefreshTokenTtl ?? TimeSpan.FromMinutes(DefaultRefreshTokenTtlInMinutes);

            return new AppUserRefreshToken
            {
                UserId = user.Id,
                LoginProvider = loginProvider,
                Name = name,
                Value = value,
                Expires = DateTime.UtcNow.Add(refreshTokenTtl),
                Created = DateTime.UtcNow
            };
        }

        protected override Task AddUserTokenAsync(AppUserRefreshToken refreshToken)
        {
            Context.RefreshTokens.Add(refreshToken ?? throw new InvalidOperationException());
            return Task.CompletedTask;
        }

        public async Task RevokeTokenAsync(AppUser user, string loginProvider, string tokenName, string tokenValue, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var storedToken = await Context.RefreshTokens.Where(x => x.UserId == user.Id && x.LoginProvider == loginProvider &&
                x.Name == tokenName && x.Value == tokenValue).FirstOrDefaultAsync(cancellationToken);
            if (storedToken == null)
            {
                return;
            }

            storedToken.Revoked = DateTime.UtcNow;
            Context.RefreshTokens.Update(storedToken);
        }

        public AppUserRefreshToken GetTokenByValue(AppUser user, string loginProvider, string tokenName, string tokenValue)
        {
            var refreshToken = Context.RefreshTokens.AsNoTracking().FirstOrDefault(x => x.UserId == user.Id &&
                x.LoginProvider == loginProvider &&
                x.Name == tokenName && x.Value == tokenValue);
            return refreshToken;
        }
    }
}
