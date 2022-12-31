using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JwtAuthServer.Authentication.Entities;
using Microsoft.AspNetCore.Identity;

namespace JwtAuthServer.Authentication.Validators
{
    public class AppUserValidator : IUserValidator<AppUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user)
        {
            if (string.IsNullOrWhiteSpace(user.Email))
            {
                return Task.FromResult(
                    IdentityResult.Failed(new IdentityError
                    {
                        Code = "EmptyEmail",
                        Description = "User's email cannot be empty."
                    }));
            }

            var isValidEmail = Regex.IsMatch(user.Email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                RegexOptions.IgnoreCase);

            if (!isValidEmail)
            {
                return Task.FromResult(
                    IdentityResult.Failed(new IdentityError
                    {
                        Code = "InvalidEmail",
                        Description = "User's email is invalid."
                    }));
            }

            return Task.FromResult(
                IdentityResult.Success);
        }
    }
}
