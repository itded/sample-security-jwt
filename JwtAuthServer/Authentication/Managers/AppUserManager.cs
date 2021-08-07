using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JwtAuthServer.Authentication.Entities;
using JwtAuthServer.Authentication.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JwtAuthServer.Authentication.Managers
{
    public class AppUserManager : UserManager<AppUser>
    {
        private readonly AppRefreshTokenUserStore _refreshTokenStore;

        public AppUserManager(IUserStore<AppUser> store, AppRefreshTokenUserStore refreshTokenStore, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<AppUser> passwordHasher, IEnumerable<IUserValidator<AppUser>> userValidators, IEnumerable<IPasswordValidator<AppUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<AppUser>> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            _refreshTokenStore = refreshTokenStore;
        }

        public async Task<IdentityResult> SetRefreshTokenAsync(AppUser user, string loginProvider, string tokenName, string tokenValue)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (loginProvider == null)
            {
                throw new ArgumentNullException(nameof(loginProvider));
            }
            if (tokenName == null)
            {
                throw new ArgumentNullException(nameof(tokenName));
            }

            await _refreshTokenStore.SetTokenAsync(user, loginProvider, tokenName, tokenValue, CancellationToken);
            return await UpdateUserAsync(user);
        }

        public async Task<IdentityResult> InvalidateRefreshTokenAsync(AppUser user, string loginProvider, string tokenName, string tokenValue)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (loginProvider == null)
            {
                throw new ArgumentNullException(nameof(loginProvider));
            }
            if (tokenName == null)
            {
                throw new ArgumentNullException(nameof(tokenName));
            }

            await _refreshTokenStore.RevokeTokenAsync(user, loginProvider, tokenName, tokenValue, CancellationToken);
            return await UpdateUserAsync(user);
        }
    }
}
