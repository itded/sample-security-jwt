using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using JwtAuth.Common.Models;
using JwtAuth.Schemes.Managers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace JwtAuth.Schemes
{
    public class CustomJwtAuthenticationHandler : AuthenticationHandler<CustomJwtAuthenticationOptions>
    {
        private const string BearerScheme = "Bearer ";

        private readonly ICustomJwtAuthenticationManager _authenticationManager;

        public CustomJwtAuthenticationHandler(IOptionsMonitor<CustomJwtAuthenticationOptions> options,
            ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, ICustomJwtAuthenticationManager authenticationManager) : base(options, logger, encoder, clock)
        {
            _authenticationManager = authenticationManager;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var authHeader = Convert.ToString(Request.Headers[HeaderNames.Authorization]);
            if (string.IsNullOrEmpty(authHeader))
            {
                return AuthenticateResult.NoResult();
            }

            var tokenJson = string.Empty;
            if (authHeader.StartsWith(BearerScheme, StringComparison.OrdinalIgnoreCase))
            {
                tokenJson = authHeader.Substring(BearerScheme.Length).Trim();
            }

            if (string.IsNullOrEmpty(tokenJson))
            {
                return AuthenticateResult.NoResult();
            }

            var request = JsonSerializer.Deserialize<ValidateTokenRequest>(tokenJson);

            // TODO: call Api to validate
            var response = await _authenticationManager.ValidateTokenAsync(Options, request);

            if (!response.Succeeded)
            {
                return AuthenticateResult.Fail("Invalid token request");
            }

            var claims = new List<Claim>
            {
                new (ClaimTypes.Name, request.UserName),
            };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new GenericPrincipal(identity, null);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            var result = AuthenticateResult.Success(ticket);
            return result;
        }
    }
}