using System.Linq;
using JwtAuthServer.Authentication.Data;
using JwtAuthServer.Authentication.Entities;
using JwtAuthServer.Authentication.Managers;
using JwtAuthServer.Authentication.Providers;
using JwtAuthServer.Authentication.Services;
using JwtAuthServer.Authentication.Stores;
using JwtAuthServer.Authentication.Validators;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace JwtAuthServer.Configuration
{
    public static class ConfigurationHelper
    {
        public static void ConfigureIdentitySystem(IServiceCollection services)
        {
            // https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.identity.identitybuilder?view=aspnetcore-5.0
            services.AddIdentity<AppUser, AppRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders()
                .AddTokenProvider<AppTokenProvider>(nameof(AppTokenProvider))
                .AddTokenProvider<AppRefreshTokenProvider>(nameof(AppRefreshTokenProvider))
                .AddUserStore<UserStore<AppUser, AppRole, AppDbContext, long, AppUserClaim, AppUserRole, AppUserLogin, AppUserToken, AppRoleClaim>>()
                .AddRoleStore<RoleStore<AppRole, AppDbContext, long, AppUserRole, AppRoleClaim>>()
                .AddUserManager<AppUserManager>()
                .AddRoleManager<AppRoleManager>()
                .AddUserValidator<AppUserValidator>();

            services.AddScoped<AppUserManager>();
            services.AddScoped<AppRoleManager>();
            services.AddScoped<IAppUserService, AppUserService>();
            services.AddScoped<IAppRoleService, AppRoleService>();

            services.AddScoped<AppRefreshTokenUserStore>();
        }

        public static void ConfigureMapping(IServiceCollection services)
        {
            // global settings because we use the Adapt method
            var config = TypeAdapterConfig.GlobalSettings;
            //config.RequireDestinationMemberSource = true;

            // scan the current assembly and register all found mappings
            var mappingRegistrations = config.Scan(typeof(Authentication.Mapping.AppUserMappingRegister).Assembly);
            mappingRegistrations.ToList().ForEach(register => register.Register(config));
            var mapperConfig = new Mapper(config);
            services.AddSingleton(config);
            services.AddSingleton<IMapper>(mapperConfig);
        }
    }
}
