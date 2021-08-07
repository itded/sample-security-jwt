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
            await _userManager.SetRefreshTokenAsync(user, nameof(AppRefreshTokenProvider), "RefreshToken", result);

            return result;
        }

        public async Task<RotateTokenResponse> RotateTokenAsync(RotateTokenRequest model)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                if (user == null)
                {
                    return new RotateTokenResponse(new ResponseError()
                    {
                        Code = "UserDoesNotExist",
                        Description = "User does not exist"
                    });
                }

                var isTokenValid = await _userManager.VerifyUserTokenAsync(user, nameof(AppTokenProvider),
                    "VerifyToken",
                    model.Token);
                if (!isTokenValid)
                {
                    return new RotateTokenResponse(new ResponseError()
                    {
                        Code = "InvalidToken",
                        Description = "Token is invalid"
                    });
                }

                var isRefreshTokenValid = await _userManager.VerifyUserTokenAsync(user, nameof(AppRefreshTokenProvider),
                    "VerifyRefreshToken",
                    model.RefreshToken);
                if (!isRefreshTokenValid)
                {
                    return new RotateTokenResponse(new ResponseError()
                    {
                        Code = "InvalidRefreshToken",
                        Description = "Refresh token is invalid"
                    });
                }

                // generate a new refresh token and invalidate the current one
                var jwtToken = await GenerateJwtTokenAsync(user);
                var refreshToken = await GenerateRefreshTokenAsync(user);

                await _userManager.InvalidateRefreshTokenAsync(user, nameof(AppRefreshTokenProvider), "RefreshToken",
                    model.RefreshToken);

                return new RotateTokenResponse()
                {
                    JwtToken = jwtToken,
                    RefreshToken = refreshToken
                };
            }
            catch (Exception ex)
            {
                return new RotateTokenResponse(new ResponseError()
                {
                    Code = "Exception",
                    Description = ex.Message
                });
            }
        }

        public async Task<ValidateTokenResponse> ValidateTokenAsync(ValidateTokenRequest model)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                if (user == null)
                {
                    return new ValidateTokenResponse(new ResponseError()
                    {
                        Code = "UserDoesNotExist",
                        Description = "User does not exist"
                    });
                }

                var isTokenValid = await _userManager.VerifyUserTokenAsync(user, nameof(AppTokenProvider),
                    "VerifyToken",
                    model.Token);
                if (!isTokenValid)
                {
                    return new ValidateTokenResponse(new ResponseError()
                    {
                        Code = "InvalidToken",
                        Description = "Token is invalid"
                    });
                }

                var isRefreshTokenValid = await _userManager.VerifyUserTokenAsync(user, nameof(AppRefreshTokenProvider),
                    "VerifyRefreshToken",
                    model.RefreshToken);
                if (!isRefreshTokenValid)
                {
                    return new ValidateTokenResponse(new ResponseError()
                    {
                        Code = "InvalidRefreshToken",
                        Description = "Refresh token is invalid"
                    });
                }

                return new ValidateTokenResponse();
            }
            catch (Exception ex)
            {
                return new ValidateTokenResponse(new ResponseError()
                {
                    Code = "Exception",
                    Description = ex.Message
                });
            }
        }
    }
}
