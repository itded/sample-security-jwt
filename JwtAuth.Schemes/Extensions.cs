using System;
using JwtAuth.Schemes.Managers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace JwtAuth.Schemes
{
    public static class Extensions
    {
        public static AuthenticationBuilder AddCustomJwtBearer(
            this AuthenticationBuilder builder,
            string authenticationScheme,
            Action<CustomJwtAuthenticationOptions> configureOptions)
        {
            builder.Services.AddScoped<ICustomJwtAuthenticationManager, CustomJwtAuthenticationManager>();
            builder.Services.AddOptions<CustomJwtAuthenticationOptions>(authenticationScheme).Validate(o => !string.IsNullOrEmpty(o.Authority), "The authority option is required.");
            return builder.AddScheme<CustomJwtAuthenticationOptions, CustomJwtAuthenticationHandler>(authenticationScheme, null, configureOptions);
        }

    }
}