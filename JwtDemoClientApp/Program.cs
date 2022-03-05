using System;
using System.Threading.Tasks;
using JwtDemoClientApp.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace JwtDemoClientApp
{
    public class Program
    {
        public static async Task Main()
        {
            // Configure the app
            var serviceProvider = CreateServiceProvider();

            // Send an authorization request to the server api
            var getTesterTokenCommand = serviceProvider.GetRequiredService<GetTesterTokenCommand>();
            var getTesterTokenCommandResult = await getTesterTokenCommand.ExecuteAsync();
            if (!getTesterTokenCommandResult.Success)
            {
                return;
            }

            // Send a request to the resource server
            var getTesterDocumentCommand = serviceProvider.GetRequiredService<GetTesterDocumentCommand>();
            var response = getTesterTokenCommandResult.Response;
            await getTesterDocumentCommand.ExecuteAsync(response.UserName, response.JwtToken);
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