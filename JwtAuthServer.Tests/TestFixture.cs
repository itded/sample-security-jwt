using System;
using System.Text;
using JwtAuthServer.Authentication.Data;
using JwtAuthServer.Configuration;
using JwtAuthServer.Settings;
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

            ConfigurationHelper.ConfigureIdentitySystem(serviceCollection);

            serviceCollection.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()));

            serviceCollection.AddLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
            });

            ConfigurationHelper.ConfigureMapping(serviceCollection);

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
    }
}
