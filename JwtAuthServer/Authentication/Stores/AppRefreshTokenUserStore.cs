using System;
using System.Threading;
using System.Threading.Tasks;
using JwtAuthServer.Authentication.Data;
using JwtAuthServer.Authentication.Entities;
using JwtAuthServer.Settings;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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
            var refreshTokenTtl = GetRefreshTokenTtl();
            var now = DateTime.Now;

            return new AppUserRefreshToken
            {
                UserId = user.Id,
                LoginProvider = loginProvider,
                Name = name,
                Value = value,
                Expires = now.Add(refreshTokenTtl),
                Created = now
            };
        }

        public override async Task SetTokenAsync(AppUser user, string loginProvider, string name, string value, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var tokenAsync = await FindTokenAsync(user, loginProvider, name, cancellationToken);
            if (tokenAsync == null)
            {
                await AddUserTokenAsync(CreateUserToken(user, loginProvider, name, value));
            }
            else
            {
                var refreshTokenTtl = GetRefreshTokenTtl();
                var now = DateTime.Now;

                tokenAsync.Revoked = null;
                tokenAsync.Value = value;
                tokenAsync.Expires = now.Add(refreshTokenTtl);
                tokenAsync.Created = now;
            }
        }

        protected override Task AddUserTokenAsync(AppUserRefreshToken refreshToken)
        {
            Context.RefreshTokens.Add(refreshToken ?? throw new InvalidOperationException());
            return Task.CompletedTask;
        }

        public async Task RevokeTokenAsync(AppUser user, string loginProvider, string tokenName, string tokenValue, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var storedToken = await FindTokenAsync(user, loginProvider, tokenName, cancellationToken);
            if (storedToken == null)
            {
                return;
            }

            storedToken.Revoked = DateTime.UtcNow;
            Context.RefreshTokens.Update(storedToken);

            await AddRevokedRefreshToken(storedToken);
        }

        private Task AddRevokedRefreshToken(AppUserRefreshToken storedToken)
        {
            var revokedRefreshToken = storedToken.Adapt<AppRevokedUserRefreshToken>();

            Context.RevokedRefreshTokens.Add(revokedRefreshToken);
            return Task.CompletedTask;
        }

        public Task<AppUserRefreshToken> GetRefreshTokenAsync(AppUser user, string loginProvider, string tokenName)
        {
            return FindTokenAsync(user, loginProvider, tokenName, CancellationToken.None);
        }

        protected override Task<AppUserRefreshToken> FindTokenAsync(
            AppUser user,
            string loginProvider,
            string name,
            CancellationToken cancellationToken)
        {
            return Context.RefreshTokens.FindAsync(new object[]
            {
                user.Id,
                loginProvider,
                name
            }, cancellationToken).AsTask();
        }

        private TimeSpan GetRefreshTokenTtl()
        {
            var settings = _config.GetSection(JwtSettings.Position).Get<JwtSettings>();
            var refreshTokenTtl = settings.RefreshTokenTtl ?? TimeSpan.FromMinutes(DefaultRefreshTokenTtlInMinutes);
            return refreshTokenTtl;
        }
    }
}
