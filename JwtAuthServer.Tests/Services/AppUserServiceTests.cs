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
        public void AddUser_ShouldBeOk()
        {
            using var scope = _testFixture.ServiceProvider.CreateScope();
            var userService = scope.ServiceProvider.GetRequiredService<IAppUserService>();

            var result = userService.RegisterUserAsync(new UserRegisterModel()
            {
                Email = "test@test.com",
                FirstName = "test-fn",
                LastName = "test-ln",
                Password = "P@ssw0rd",
                UserName = "test"
            }).Result;

            Assert.True(result.Succeeded);
        }

        [Fact]
        public void AddUser_InvalidEmail_ShouldFail()
        {
            using var scope = _testFixture.ServiceProvider.CreateScope();
            var userService = scope.ServiceProvider.GetRequiredService<IAppUserService>();

            var result = userService.RegisterUserAsync(new UserRegisterModel()
            {
                Email = "testtest.com",
                FirstName = "test-fn",
                LastName = "test-ln",
                Password = "P@ssw0rd",
                UserName = "test"
            }).Result;

            Assert.False(result.Succeeded);
            Assert.Contains(result.Errors, x => x.Code.Equals("InvalidEmail"));
        }
    }
}
