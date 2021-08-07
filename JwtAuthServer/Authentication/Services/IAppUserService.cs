using System.Threading.Tasks;
using JwtAuthServer.Authentication.Models;

namespace JwtAuthServer.Authentication.Services
{
    public interface IAppUserService
    {
        Task<UserRegisterResponse> RegisterUserAsync(UserRegisterRequest model);

        Task<UserLoginResponse> LoginUserAsync(UserLoginRequest model);

        Task<RotateTokenResponse> RotateTokenAsync(RotateTokenRequest model);

        Task<ValidateTokenResponse> ValidateTokenAsync(ValidateTokenRequest model);
    }
}
