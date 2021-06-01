using System;
using System.Threading.Tasks;
using JwtAuthServer.Authentication.Entities;
using JwtAuthServer.Authentication.Managers;
using JwtAuthServer.Authentication.Models;
using JwtAuthServer.Authentication.Providers;
using Mapster;

namespace JwtAuthServer.Authentication.Services
{
    public class AppUserService : IAppUserService
    {
        private readonly AppUserManager _userManager;

        public AppUserService(AppUserManager userManager)
        {
            _userManager = userManager;
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
            var result = await _userManager.GenerateUserTokenAsync(user, nameof(AppTokenProvider), "Token");
            return result;
        }
    }
}
