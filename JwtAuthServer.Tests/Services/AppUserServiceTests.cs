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

        [Fact]
        public async Task RotateToken_UserDoesNotExist_ShouldFail()
        {
            const string userName = "test";
            const string token = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiIxIiwidW5pcXVlX25hbWUiOiJ0ZXN0IiwibmJmIjoxNjI4MzQ2ODAwLCJleHAiOjE2MjgzNDcxMDAsImlhdCI6MTYyODM0NjgwMH0.Z9uwD0hO573-GLAV7WfKZ31wEpE8_Jl8ObIct-y7mOHCpDO09wOsvqqiysbKL0QZ9nqWJmRlbL8OPJdzEix4cw";
            const string refreshToken = "XjO7/sGgYjh79iJds7bQ/W0yAqwsFH4r2lFRj6jDt4I=";

            using var scope = _testFixture.ServiceProvider.CreateScope();
            var userService = scope.ServiceProvider.GetRequiredService<IAppUserService>();

            var result = await userService.RotateTokenAsync(new RotateTokenRequest()
            {
                UserName = userName,
                Token = token,
                RefreshToken = refreshToken
            });

            Assert.False(result.Succeeded);
            Assert.Contains(result.Errors, x => x.Code.Equals("UserDoesNotExist"));
        }

        [Fact]
        public async Task RotateToken_VerifyUser_InvalidRefreshToken_ShouldFail()
        {
            const string userName = "test";
            const string password = "P@ssw0rd";
            const string rndRefreshToken = "XjO7/sGgYjh79iJds7bQ/W0yAqwsFH4r2lFRj6jDt4I=";

            using var scope = _testFixture.ServiceProvider.CreateScope();
            var userService = scope.ServiceProvider.GetRequiredService<IAppUserService>();

            await userService.RegisterUserAsync(new UserRegisterRequest()
            {
                Email = "test@test.com",
                FirstName = "test-fn",
                LastName = "test-ln",
                UserName = userName,
                Password = password
            });

            var loginResult = await userService.LoginUserAsync(new UserLoginRequest()
            {
                UserName = userName,
                Password = password
            });

            var result = await userService.RotateTokenAsync(new RotateTokenRequest()
            {
                UserName = loginResult.UserInfo.UserName,
                Token = loginResult.JwtToken,
                RefreshToken = rndRefreshToken
            });

            Assert.False(result.Succeeded);
            Assert.Contains(result.Errors, x => x.Code.Equals("InvalidRefreshToken"));
        }

        [Fact]
        public async Task RotateToken_VerifyUser_ShouldBeOk()
        {
            const string userName = "test";
            const string password = "P@ssw0rd";

            using var scope = _testFixture.ServiceProvider.CreateScope();
            var userService = scope.ServiceProvider.GetRequiredService<IAppUserService>();

            await userService.RegisterUserAsync(new UserRegisterRequest()
            {
                Email = "test@test.com",
                FirstName = "test-fn",
                LastName = "test-ln",
                UserName = userName,
                Password = password
            });

            var loginResult = await userService.LoginUserAsync(new UserLoginRequest()
            {
                UserName = userName,
                Password = password
            });

            var rotateResult = await userService.RotateTokenAsync(new RotateTokenRequest()
            {
                UserName = loginResult.UserInfo.UserName,
                Token = loginResult.JwtToken,
                RefreshToken = loginResult.RefreshToken
            });

            Assert.True(rotateResult.Succeeded);
            Assert.NotEqual(rotateResult.JwtToken, loginResult.JwtToken);
            Assert.NotEqual(rotateResult.RefreshToken, loginResult.RefreshToken);
        }
    }
}
