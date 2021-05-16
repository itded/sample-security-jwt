using System;

namespace JwtAuthServer.Settings
{
    public class JwtSettings
    {
        public string Secret { get; set; }

        public TimeSpan TokenTtl { get; set; }

        public TimeSpan? RefreshTokenTtl { get; set; }
    }
}
