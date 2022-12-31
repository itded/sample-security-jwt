using JwtAuthServer.Authentication.Data;
using JwtAuthServer.Configuration;
using JwtAuthServer.RegistrationTool.Factories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NLog.Extensions.Logging;

namespace JwtAuthServer.RegistrationTool
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public Startup()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton((IConfiguration)Configuration);
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddNLog(Configuration);
                loggingBuilder.SetMinimumLevel(LogLevel.Debug);
            });

            // options
            NLog.LogManager.Configuration = new NLogLoggingConfiguration(this.Configuration.GetSection("NLog"));

            ConfigurationHelper.ConfigureIdentitySystem(services);

            services.AddDbContext<AppDbContext>
                (options => options.UseSqlServer(Configuration.GetConnectionString("AuthConnectionString")));

            ConfigurationHelper.ConfigureMapping(services);

            services.AddSingleton<ICommandFactory, CommandFactory>();
            // do not want to work with tokens in the tool
            services.AddSingleton<TokenValidationParameters>(_ => null);
        }
    }
}
