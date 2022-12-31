using System.Threading.Tasks;
using JwtAuthServer.Authentication.Models;

namespace JwtAuthServer.Authentication.Services
{
    public interface IAppRoleService
    {
        Task<IdentityRolesResponse> AddIdentityRolesAsync(IdentityRolesRequest model);
    }
}
