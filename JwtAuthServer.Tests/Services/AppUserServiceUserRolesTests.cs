using System.Threading.Tasks;
using JwtAuthServer.Authentication.Models;
using JwtAuthServer.Authentication.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace JwtAuthServer.Tests.Services
{
    public class AppUserServiceUserRolesTests: IClassFixture<TestFixture>
    {
        private readonly TestFixture _testFixture;

        public AppUserServiceUserRolesTests(TestFixture testFixture)
        {
            _testFixture = testFixture;
        }

        [Fact]
        public async Task AddUserRole_ShouldBeOk()
        {
            using var scope = _testFixture.ServiceProvider.CreateScope();
            var userService = scope.ServiceProvider.GetRequiredService<IAppUserService>();
            var roleService = scope.ServiceProvider.GetRequiredService<IAppRoleService>();

            var registerUserResult = await userService.RegisterUserAsync(new UserRegisterRequest()
            {
                Email = "test@test.com",
                FirstName = "test-fn",
                LastName = "test-ln",
                Password = "P@ssw0rd",
                UserName = "test"
            });

            Assert.True(registerUserResult.Succeeded);

            var addIdentityRolesResult = await roleService.AddIdentityRolesAsync(new IdentityRolesRequest()
            {
                IdentityRoles = new[]
                {
                    new IdentityRolesRequest.IdentityRole()
                    {
                        RoleName = "Admin",
                        Description = "Admin Role"
                    },
                    new IdentityRolesRequest.IdentityRole()
                    {
                        RoleName = "User",
                        Description = "User Role"
                    },
                    new IdentityRolesRequest.IdentityRole()
                    {
                        RoleName = "Tester",
                        Description = "Tester Role"
                    }
                },
                ContinueOnError = false
            });

            Assert.True(addIdentityRolesResult.Succeeded);

            var addUserToRolesResult = await userService.AddUserToRolesAsync(new AddUserToRolesRequest()
            {
                UserName = "test",
                RoleNames = new[] { "Admin", "Tester" }
            });

            Assert.True(addUserToRolesResult.Succeeded);
        }
    }
}
