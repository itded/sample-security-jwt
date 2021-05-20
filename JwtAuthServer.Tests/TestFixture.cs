using System;
using JwtAuthServer.Authentication.Data;
using JwtAuthServer.Authentication.Entities;
using JwtAuthServer.Authentication.Managers;
using JwtAuthServer.Authentication.Services;
using JwtAuthServer.Authentication.Validators;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace JwtAuthServer.Tests
{
    public class TestFixture
    {
        public TestFixture()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()));
            serviceCollection.AddIdentity<AppUser, AppRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders()
                .AddUserStore<UserStore<AppUser, AppRole, AppDbContext, long>>()
                .AddRoleStore<RoleStore<AppRole, AppDbContext, long>>()
                .AddUserManager<AppUserManager>()
                .AddUserValidator<AppUserValidator>();

            serviceCollection.AddLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
            });

            serviceCollection.AddScoped<AppUserManager>();
            serviceCollection.AddScoped<IAppUserService, AppUserService>();

            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        public ServiceProvider ServiceProvider { get; private set; }
    }
}
