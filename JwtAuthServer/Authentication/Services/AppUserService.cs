using System;
using System.Threading.Tasks;
using JwtAuthServer.Authentication.Entities;
using JwtAuthServer.Authentication.Managers;
using JwtAuthServer.Authentication.Models;
using Microsoft.AspNetCore.Identity;

namespace JwtAuthServer.Authentication.Services
{
    public class AppUserService : IAppUserService
    {
        private readonly AppUserManager _userManager;

        public AppUserService(AppUserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task<IdentityResult> RegisterUserAsync(UserRegisterModel model)
        {
            var userExists = await _userManager.FindByNameAsync(model.UserName);
            if (userExists != null)
            {
                return IdentityResult.Failed(new IdentityError()
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

            return await _userManager.CreateAsync(user, model.Password);
        }
    }
}
