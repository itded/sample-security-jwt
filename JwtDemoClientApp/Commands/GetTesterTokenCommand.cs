using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using JwtAuth.Common.Models;
using Microsoft.Extensions.Logging;

namespace JwtDemoClientApp.Commands
{
    public class GetTesterTokenCommand
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<GetTesterTokenCommand> _logger;

        public GetTesterTokenCommand(IHttpClientFactory httpClientFactory, ILogger<GetTesterTokenCommand> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<(string Token, bool Success)> ExecuteAsync()
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient(Constants.ServerApiClient);

                var request = new
                {
                    UserName = "tester",
                    Password = "P@ssw0rd!"
                };

                var response = await httpClient.PostAsJsonAsync("/auth/authenticate", request, CancellationToken.None);

                if (response.IsSuccessStatusCode)
                {
                    var userLoginResponse = await response.Content.ReadFromJsonAsync<UserLoginResponse>();
                    return (userLoginResponse.JwtToken, true);
                }
                else
                {
                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Failed with Code:{response.StatusCode}. Content: {content}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return (null, false);
        }
    }
}