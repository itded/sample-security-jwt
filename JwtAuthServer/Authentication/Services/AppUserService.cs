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
            var refreshToken = await GenerateRefreshTokenAsync(user);

            return new UserLoginResponse()
            {
                UserInfo = user.Adapt<UserInfo>(),
                JwtToken = jwtToken,
                RefreshToken = refreshToken
            };
        }

        private async Task<string> GenerateJwtTokenAsync(AppUser user)
        {
            var result = await _userManager.GenerateUserTokenAsync(user, nameof(AppTokenProvider), "Token");
            return result;
        }

        private async Task<string> GenerateRefreshTokenAsync(AppUser user)
        {
            var result = await _userManager.GenerateUserTokenAsync(user, nameof(AppRefreshTokenProvider), "RefreshToken");
            await _userManager.SetAuthenticationTokenAsync(user, nameof(AppRefreshTokenProvider), "RefreshToken", result);

            return result;
        }

        // private async Task<VerifyTokenResponse> VerifyTokenAsync(VerifyTokenRequest tokenRequest)
        // {
        //     IUserAuthenticationTokenStore<AppUserToken> store;
        //     _userManager.GenerateUserTokenAsync()
        // }


        // private async Task<VerifyTokenResponse> VerifyTokenAsync(VerifyTokenRequest tokenRequest)
        // {
        //     try
        //     {
        //         // validation 1 - general
        //         var jwtTokenHandler = new JwtSecurityTokenHandler();
        //         var jwtClaimsPrincipal = jwtTokenHandler.ValidateToken(tokenRequest.Token, _tokenValidationParameters,
        //             out var validatedToken);
        //
        //         // validation 2 - algorithm
        //         if (validatedToken is JwtSecurityToken jwtSecurityToken)
        //         {
        //             var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithm,
        //                 StringComparison.InvariantCultureIgnoreCase);
        //
        //             if (result == false)
        //             {
        //                 return null;
        //             }
        //         }
        //         else
        //         {
        //             return null;
        //         }
        //
        //         // validation 3 - expiration date
        //
        //         // validation 4 - validate existence of the token
        //         _userManager.VerifyUserTokenAsync()
        //         var storedToken =
        //             await _apiDbContext.RefreshTokens.FirstOrDefaultAsync(x => x.Token == tokenRequest.RefreshToken);
        //
        //     }
        //     catch (Exception ex)
        //     {
        //     }
        // }
    }
}
