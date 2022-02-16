using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace JwtDemoClientApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Send an authorization request to the server api

            var host = CreateHostBuilder(args).Build();

            try
            {
                var httpClientFactory = host.Services.GetRequiredService<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient("DemoClient");

                var request = new
                {
                    UserName = "tester",
                    Password = "P@ssw0rd!"
                };

                var response = await httpClient.PostAsJsonAsync("/auth/authenticate", request, CancellationToken.None);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(content);
                }
                else
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Failed with Code:{response.StatusCode}. Content: {content}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHttpClient("DemoClient", httpClient =>
                    {
                        httpClient.BaseAddress = new Uri("https://localhost:5001");
                    }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                    {
                        ClientCertificateOptions = ClientCertificateOption.Manual,
                        ServerCertificateCustomValidationCallback =
                            (_, _, _, _) => true
                    });
                });
    }
}