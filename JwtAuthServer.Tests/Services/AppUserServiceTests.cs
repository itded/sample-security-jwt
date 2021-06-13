using System.Threading.Tasks;
using JwtAuthServer.Authentication.Models;
using JwtAuthServer.Authentication.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace JwtAuthServer.Tests.Services
{
    public class AppUserServiceTests : IClassFixture<TestFixture>
    {
        private readonly TestFixture _testFixture;

        public AppUserServiceTests(TestFixture testFixture)
        {
            _testFixture = testFixture;
        }

        [Fact]
        public async Task AddUser_ShouldBeOk()
        {
            using var scope = _testFixture.ServiceProvider.CreateScope();
            var userService = scope.ServiceProvider.GetRequiredService<IAppUserService>();

            var result = await userService.RegisterUserAsync(new UserRegisterRequest()
            {
                Email = "test@test.com",
                FirstName = "test-fn",
                LastName = "test-ln",
                Password = "P@ssw0rd",
                UserName = "test"
            });

            Assert.True(result.Succeeded);
        }

        [Fact]
        public async Task AddUser_InvalidEmail_ShouldFail()
        {
            using var scope = _testFixture.ServiceProvider.CreateScope();
            var userService = scope.ServiceProvider.GetRequiredService<IAppUserService>();

            var result = await userService.RegisterUserAsync(new UserRegisterRequest()
            {
                Email = "testtest.com",
                FirstName = "test-fn",
                LastName = "test-ln",
                Password = "P@ssw0rd",
                UserName = "test"
            });

            Assert.False(result.Succeeded);
            Assert.Contains(result.Errors, x => x.Code.Equals("InvalidEmail"));
        }

        [Fact]
        public async Task LoginUser_ShouldBeOk()
        {
            const string userName = "test";
            const string password = "P@ssw0rd";

            using var scope = _testFixture.ServiceProvider.CreateScope();
            var userService = scope.ServiceProvider.GetRequiredService<IAppUserService>();

            var result = await userService.RegisterUserAsync(new UserRegisterRequest()
            {
                Email = "test@test.com",
                FirstName = "test-fn",
                LastName = "test-ln",
                UserName = userName,
                Password = password
            });

            Assert.True(result.Succeeded);

            var loginResult = await userService.LoginUserAsync(new UserLoginRequest()
            {
                UserName = userName,
                Password = password
            });

            Assert.True(loginResult.Succeeded);
            Assert.NotNull(loginResult.UserInfo);
            Assert.NotEmpty(loginResult.JwtToken);
            Assert.NotEmpty(loginResult.RefreshToken);
        }
    }
}
