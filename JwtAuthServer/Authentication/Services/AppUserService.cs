using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using JwtAuthServer.Authentication.Entities;
using JwtAuthServer.Authentication.Managers;
using JwtAuthServer.Authentication.Models;
using JwtAuthServer.Settings;
using Mapster;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace JwtAuthServer.Authentication.Services
{
    public class AppUserService : IAppUserService
    {
        private readonly AppUserManager _userManager;
        private readonly IConfiguration _config;

        public AppUserService(AppUserManager userManager, IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
        }

        public async Task<UserRegisterResponse> RegisterUserAsync(UserRegisterRequest model)
        {
            var userExists = await _userManager.FindByNameAsync(model.UserName);
            if (userExists != null)
            {
                return new UserRegisterResponse(new ResponseError()
                {
                    Code = "UserExists",
                    Description = "User already exists"
                });
            }

            var user = new AppUser(model.FirstName, model.LastName, model.UserName)
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var createUserResult = await _userManager.CreateAsync(user, model.Password);
            var userRegisterResponse = createUserResult.Adapt<UserRegisterResponse>();
            return userRegisterResponse;
        }

        public async Task<UserLoginResponse> LoginUserAsync(UserLoginRequest model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                return new UserLoginResponse(new ResponseError()
                {
                    Code = "UserDoesNotExist",
                    Description = "User does not exist"
                });
            }

            if (!await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return new UserLoginResponse(new ResponseError()
                {
                    Code = "UserDoesNotExist",
                    Description = "User does not exist"
                });
            }

            var jwtToken = await GenerateJwtTokenAsync(user);

            return new UserLoginResponse()
            {
                UserInfo = user.Adapt<UserInfo>(),
                JwtToken = jwtToken
            };
        }

        private async Task<string> GenerateJwtTokenAsync(AppUser user)
        {
            var claims = new List<Claim>
            {
                new (ClaimTypes.NameIdentifier, user.Id.ToString()),
                new (ClaimTypes.Name, user.UserName),
            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var settings = _config.GetSection(JwtSettings.Position).Get<JwtSettings>();
            var tokenSecret = settings.Secret;
            var tokenTtl = settings.TokenTtl;
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSecret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);

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
    }
}
