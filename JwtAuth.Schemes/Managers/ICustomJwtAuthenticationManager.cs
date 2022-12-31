using System.Threading.Tasks;
using JwtAuth.Common.Models;

namespace JwtAuth.Schemes.Managers
{
    public interface ICustomJwtAuthenticationManager
    {
        Task<ValidateTokenResponse> ValidateTokenAsync(CustomJwtAuthenticationOptions options, ValidateTokenRequest request);
    }
}