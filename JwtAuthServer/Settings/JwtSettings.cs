using System;

namespace JwtAuthServer.Settings
{
    public class JwtSettings
    {
        public const string Position = "JWT";

        public string Secret { get; set; }

        public TimeSpan TokenTtl { get; set; }

        public TimeSpan? RefreshTokenTtl { get; set; }
    }
}
