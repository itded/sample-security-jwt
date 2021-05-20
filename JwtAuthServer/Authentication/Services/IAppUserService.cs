using System.Threading.Tasks;
using JwtAuthServer.Authentication.Models;
using Microsoft.AspNetCore.Identity;

namespace JwtAuthServer.Authentication.Services
{
    public interface IAppUserService
    {
        Task<IdentityResult> RegisterUserAsync(UserRegisterModel model);
    }
}