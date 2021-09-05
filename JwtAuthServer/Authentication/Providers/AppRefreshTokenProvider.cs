using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using JwtAuthServer.Authentication.Entities;
using JwtAuthServer.Authentication.Stores;
using Microsoft.AspNetCore.Identity;

namespace JwtAuthServer.Authentication.Providers
{
    public class AppRefreshTokenProvider : IUserTwoFactorTokenProvider<AppUser>
    {
        private readonly AppRefreshTokenUserStore _refreshTokenUserStore;

        public AppRefreshTokenProvider(AppRefreshTokenUserStore refreshTokenUserStore)
        {
            _refreshTokenUserStore = refreshTokenUserStore;
        }

        public Task<string> GenerateAsync(string purpose, UserManager<AppUser> manager, AppUser user)
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            var token = Convert.ToBase64String(randomNumber);
            return ValueTask.FromResult(token).AsTask();
        }

        public async Task<bool> ValidateAsync(string purpose, string token, UserManager<AppUser> manager, AppUser user)
        {
            var refreshToken = await _refreshTokenUserStore.GetRefreshTokenAsync(
                user, nameof(AppRefreshTokenProvider), "RefreshToken");
            if (refreshToken != null && refreshToken.Value == token && refreshToken.Revoked == null && refreshToken.Expires > DateTime.UtcNow)
            {
                return true;
            }

            return  false;
        }

        public Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<AppUser> manager, AppUser user)
        {
            return Task.FromResult(false);
        }
    }
}
