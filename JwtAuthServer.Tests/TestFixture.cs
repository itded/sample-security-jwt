using System;
using System.Linq;
using JwtAuthServer.Authentication.Data;
using JwtAuthServer.Authentication.Entities;
using JwtAuthServer.Authentication.Managers;
using JwtAuthServer.Authentication.Services;
using JwtAuthServer.Authentication.Validators;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace JwtAuthServer.Tests
{
    public class TestFixture
    {
        public TestFixture()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton<IConfiguration>(GetIConfigurationRoot());

            serviceCollection.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()));
            serviceCollection.AddIdentity<AppUser, AppRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders()
                /* depends on AppDbContext configuration */
                .AddUserStore<UserStore<AppUser, AppRole, AppDbContext, long, AppUserClaim, AppUserRole, AppUserLogin, AppUserToken, AppRoleClaim>>()
                .AddRoleStore<RoleStore<AppRole, AppDbContext, long, AppUserRole, AppRoleClaim>>()
                .AddUserManager<AppUserManager>()
                .AddUserValidator<AppUserValidator>();

            serviceCollection.AddLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
            });

            RegisterMapping(serviceCollection);

            serviceCollection.AddScoped<AppUserManager>();
            serviceCollection.AddScoped<IAppUserService, AppUserService>();

            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        public ServiceProvider ServiceProvider { get; }

        private static IConfigurationRoot GetIConfigurationRoot()
        {
            var outputPath = AppDomain.CurrentDomain.BaseDirectory;
            return new ConfigurationBuilder()
                .SetBasePath(outputPath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
        }

        private void RegisterMapping(IServiceCollection services)
        {
            var config = new TypeAdapterConfig();
            // scan the current assembly and register all found mappings
            var mappingRegistrations = TypeAdapterConfig.GlobalSettings.Scan(typeof(JwtAuthServer.Authentication.Mapping.AppUserMappingRegister).Assembly);
            mappingRegistrations.ToList().ForEach(register => register.Register(config));
            var mapperConfig = new Mapper(config);
            services.AddSingleton<IMapper>(mapperConfig);
        }
    }
}
