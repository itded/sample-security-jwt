using System;
using System.Linq;
using System.Reflection;
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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace JwtAuthServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.identity.identitybuilder?view=aspnetcore-5.0
            services.AddIdentity<AppUser, AppRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders()
                .AddTokenProvider<AppTokenProvider>(nameof(AppTokenProvider))
                .AddTokenProvider<AppRefreshTokenProvider>(nameof(AppRefreshTokenProvider))
                .AddUserStore<UserStore<AppUser, AppRole, AppDbContext, long>>()
                .AddRoleStore<RoleStore<AppRole, AppDbContext, long>>()
                .AddUserManager<AppUserManager>()
                .AddUserValidator<AppUserValidator>();

            services.AddScoped<AppUserManager>();
            services.AddScoped<IAppUserService, AppUserService>();

            services.AddScoped<AppRefreshTokenUserStore>();

            services.AddDbContext<AppDbContext>
                (options => options.UseSqlServer(Configuration.GetConnectionString("AuthConnectionString")));

            services.AddLogging();

            services.AddControllers();

            RegisterMapping(services);

            // configure Jwt settings
            var jwtSettingsSection = Configuration.GetSection(JwtSettings.Position);
            var jwtSettings = jwtSettingsSection.Get<JwtSettings>();

            var tokenValidationParameters = new TokenValidationParameters()
            {
                ClockSkew = TimeSpan.FromSeconds(5),
                ValidateIssuerSigningKey = true,
                // TODO: validate the issuer and the audience
                ValidateIssuer = false,
                ValidateAudience = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
            };

            services.AddSingleton(_ => tokenValidationParameters);

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = tokenValidationParameters;
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void RegisterMapping(IServiceCollection services)
        {
            var config = new TypeAdapterConfig();
            // scan the current assembly and register all found mappings
            var mappingRegistrations = TypeAdapterConfig.GlobalSettings.Scan(typeof(Startup).Assembly);
            mappingRegistrations.ToList().ForEach(register => register.Register(config));
            var mapperConfig = new Mapper(config);
            services.AddSingleton<IMapper>(mapperConfig);
        }
    }
}
