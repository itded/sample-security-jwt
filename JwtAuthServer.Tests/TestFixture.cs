using System;
using System.Linq;
using System.Text;
using JwtAuthServer.Authentication.Data;
using JwtAuthServer.Authentication.Entities;
using JwtAuthServer.Authentication.Managers;
using JwtAuthServer.Authentication.Providers;
using JwtAuthServer.Authentication.Services;
using JwtAuthServer.Authentication.Stores;
using JwtAuthServer.Authentication.Validators;
using JwtAuthServer.Settings;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace JwtAuthServer.Tests
{
    public class TestFixture
    {
        public TestFixture()
        {
            var serviceCollection = new ServiceCollection();

            Configuration = GetIConfigurationRoot();

            serviceCollection.AddSingleton(Configuration);
            serviceCollection.AddSingleton(_ =>
                {
                    var jwtSettings = Configuration.GetSection(JwtSettings.Position).Get<JwtSettings>();
                    return new TokenValidationParameters()
                    {
                        ClockSkew = TimeSpan.FromSeconds(5),
                        ValidateIssuerSigningKey = true,
                        // TODO: validate the issuer and the audience
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
                    };
                }
            );

            serviceCollection.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()));
            serviceCollection.AddIdentity<AppUser, AppRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders()
                .AddTokenProvider<AppTokenProvider>(nameof(AppTokenProvider))
                .AddTokenProvider<AppRefreshTokenProvider>(nameof(AppRefreshTokenProvider))
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
            serviceCollection.AddScoped<AppRefreshTokenUserStore>();

            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        public ServiceProvider ServiceProvider { get; }

        public IConfiguration Configuration { get; }

        private static IConfiguration GetIConfigurationRoot()
        {
            var outputPath = AppDomain.CurrentDomain.BaseDirectory;
            return new ConfigurationBuilder()
                .SetBasePath(outputPath)
                .AddJsonFile("appsettings.json")
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
