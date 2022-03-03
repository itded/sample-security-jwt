using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using JwtAuth.Common.Models;

namespace JwtAuth.Schemes.Managers
{
    public class CustomJwtAuthenticationManager : ICustomJwtAuthenticationManager
    {
        public async Task<ValidateTokenResponse> ValidateTokenAsync(CustomJwtAuthenticationOptions options, ValidateTokenRequest request)
        {
            var httpClient = CreateHttpClient(options);

            var result = await httpClient.PostAsJsonAsync(options.ValidatePath, request);
            if (result.IsSuccessStatusCode)
            {
                return await result.Content.ReadFromJsonAsync<ValidateTokenResponse>();
            }

            return new ValidateTokenResponse()
            {
                Succeeded = false
            };
        }

        private HttpClient CreateHttpClient(CustomJwtAuthenticationOptions options)
        {
            HttpClient httpClient;

            if (options.DisableServerCertificateValidation)
            {
                var clientHandler = new HttpClientHandler()
                {
                    ClientCertificateOptions = ClientCertificateOption.Manual,
                    ServerCertificateCustomValidationCallback = (_, _, _, _) => true
                };

                httpClient = new HttpClient(clientHandler, false);
            }
            else
            {
                httpClient = new HttpClient();
            }

            httpClient.BaseAddress = new Uri(options.Authority);
            return httpClient;
        }
    }
}