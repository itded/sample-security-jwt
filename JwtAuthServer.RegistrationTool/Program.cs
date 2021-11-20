using System;
using System.Threading;
using System.Threading.Tasks;
using JwtAuthServer.Authentication.Data;
using JwtAuthServer.Authentication.Services;
using JwtAuthServer.RegistrationTool.Factories;
using JwtAuthServer.RegistrationTool.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace JwtAuthServer.RegistrationTool
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            try
            {
                ArgValidator.StringReadValidation(args);
            }
            catch (ApplicationException validationException)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(validationException.Message);
                Console.ForegroundColor = ConsoleColor.White;
                return 1;
            }

            var serviceProvider = CreateServiceProvider();
            
            InstallDb(serviceProvider);
            
            var commandFactory = serviceProvider.GetRequiredService<ICommandFactory>();

            using var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            var command = commandFactory.CreateRegistrationCommand(args);
            await command.ExecuteAsync(token);

            return 0;
        }

        private static void InstallDb(IServiceProvider serviceProvider)
        {
            try
            {
                var context = serviceProvider.GetRequiredService<AppDbContext>();
                context.Database.Migrate(); // apply all migrations
            }
            catch (Exception ex)
            {
                var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred during migrating the database");
            }
        }

        private static IServiceProvider CreateServiceProvider()
        {
            var services = new ServiceCollection();

            var startup = new Startup();
            startup.ConfigureServices(services);

            var serviceProvider = services.BuildServiceProvider();
            return serviceProvider;
        }
    }
}
