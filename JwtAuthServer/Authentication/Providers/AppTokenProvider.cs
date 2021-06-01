using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using JwtAuthServer.Authentication.Entities;
using JwtAuthServer.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace JwtAuthServer.Authentication.Providers
{
    public class AppTokenProvider: IUserTwoFactorTokenProvider<AppUser>
    {
        private const string SecurityAlgorithm = SecurityAlgorithms.HmacSha512Signature;

        private readonly IConfiguration _config;
        private readonly TokenValidationParameters _tokenValidationParameters;

        public AppTokenProvider(IConfiguration config, TokenValidationParameters tokenValidationParameters)
        {
            _config = config;
            _tokenValidationParameters = tokenValidationParameters;
        }

        public async Task<string> GenerateAsync(string purpose, UserManager<AppUser> userManager, AppUser user)
        {
            var claims = new List<Claim>
            {
                new (ClaimTypes.NameIdentifier, user.Id.ToString()),
                new (ClaimTypes.Name, user.UserName),
            };

            var roles = await userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var settings = _config.GetSection(JwtSettings.Position).Get<JwtSettings>();
            var tokenSecret = settings.Secret;
            var tokenTtl = settings.TokenTtl;
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSecret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithm);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(tokenTtl),
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public Task<bool> ValidateAsync(string purpose, string token, UserManager<AppUser> manager, AppUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var _ = jwtTokenHandler.ValidateToken(token, _tokenValidationParameters,
                out var validatedToken);

            if (validatedToken is JwtSecurityToken jwtSecurityToken)
            {
                var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithm,
                    StringComparison.InvariantCultureIgnoreCase);
                return ValueTask.FromResult(result).AsTask();
            }

            return ValueTask.FromResult(false).AsTask();
        }

        public Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<AppUser> manager, AppUser user)
        {
            return ValueTask.FromResult(false).AsTask();
        }
    }
}
