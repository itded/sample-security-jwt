using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using JwtAuthServer.Authentication.Entities;
using Microsoft.AspNetCore.Identity;

namespace JwtAuthServer.Authentication.Providers
{
    public class AppRefreshTokenProvider : IUserTwoFactorTokenProvider<AppUser>
    {
        public Task<string> GenerateAsync(string purpose, UserManager<AppUser> manager, AppUser user)
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            var token = Convert.ToBase64String(randomNumber);
            return ValueTask.FromResult(token).AsTask();
        }

        public Task<bool> ValidateAsync(string purpose, string token, UserManager<AppUser> manager, AppUser user)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<AppUser> manager, AppUser user)
        {
            return Task.FromResult(false);
        }
    }
}
