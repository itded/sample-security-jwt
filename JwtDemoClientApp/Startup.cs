using System;
using System.Net.Http;
using JwtDemoClientApp.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace JwtDemoClientApp
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
            // http clients
            services.AddHttpClient(Constants.ServerApiClient, httpClient =>
            {
                httpClient.BaseAddress = new Uri("https://localhost:5001");
            }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback =
                    (_, _, _, _) => true
            });

            services.AddHttpClient(Constants.DocumentWebClient, httpClient =>
            {
                httpClient.BaseAddress = new Uri("https://localhost:5021");
            }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback =
                    (_, _, _, _) => true
            });
            
            // logging
            services.AddSingleton((IConfiguration)Configuration);
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddNLog(Configuration);
                loggingBuilder.SetMinimumLevel(LogLevel.Debug);
            });

            // options
            NLog.LogManager.Configuration = new NLogLoggingConfiguration(this.Configuration.GetSection("NLog"));

            services.AddScoped<GetTesterTokenCommand>();
            services.AddScoped<GetTesterDocumentCommand>();
        }
    }
}