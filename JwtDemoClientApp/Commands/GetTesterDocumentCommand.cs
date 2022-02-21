using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace JwtDemoClientApp.Commands
{
    public class GetTesterDocumentCommand
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<GetTesterDocumentCommand> _logger;

        public GetTesterDocumentCommand(IHttpClientFactory httpClientFactory, ILogger<GetTesterDocumentCommand> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task ExecuteAsync(string jwtToken)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient(Constants.DocumentWebClient);
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", jwtToken); // JwtBearerDefaults.AuthenticationScheme
                await httpClient.GetAsync("/api/documents/GetAllDocumentsByRole");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }
}