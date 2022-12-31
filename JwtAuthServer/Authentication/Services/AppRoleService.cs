using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JwtAuthServer.Authentication.Entities;
using JwtAuthServer.Authentication.Managers;
using JwtAuthServer.Authentication.Models;
using Mapster;

namespace JwtAuthServer.Authentication.Services
{
    public class AppRoleService : IAppRoleService
    {
        private readonly AppRoleManager _roleManager;

        public AppRoleService(AppRoleManager roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<IdentityRolesResponse> AddIdentityRolesAsync(IdentityRolesRequest model)
        {
            var identityRoles = model.IdentityRoles;
            if (identityRoles == null || !identityRoles.Any())
            {
                return new IdentityRolesResponse(new ResponseError()
                {
                    Code = "EmptyIdentityRolesCollection",
                    Description = "The request should contain at least one identity role."
                });
            }

            if (identityRoles.Any(x => string.IsNullOrWhiteSpace(x.RoleName)))
            {
                return new IdentityRolesResponse(new ResponseError()
                {
                    Code = "EmptyIdentityRoleNames",
                    Description = "The request should not contain an empty identity role."
                });
            }

            var addedRoleNames = new List<string>();
            var existingRoleNames = new List<string>();
            var errorRoleNames = new List<string>();

            var response = new IdentityRolesResponse();

            foreach (var identityRole in identityRoles)
            {
                var roleName = identityRole.RoleName;
                var description = identityRole.Description;
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    var identityResult = await _roleManager.CreateAsync(new AppRole(roleName, description));
                    if (identityResult.Succeeded)
                    {
                        addedRoleNames.Add(roleName);
                    }
                    else
                    {
                        errorRoleNames.Add(roleName);
                        if (!model.ContinueOnError)
                        {
                            response = identityResult.Adapt<IdentityRolesResponse>();
                            break;
                        }
                    }
                }
                else
                {
                    existingRoleNames.Add(roleName);
                }
            }

            response.AddedRoleNames = addedRoleNames.ToArray();
            response.ExistingRoleNames = existingRoleNames.ToArray();
            response.ErrorRoleNames = errorRoleNames.ToArray();
            return response;
        }
    }
}
