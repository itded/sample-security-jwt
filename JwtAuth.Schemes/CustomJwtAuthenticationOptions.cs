using Microsoft.AspNetCore.Authentication;

namespace JwtAuth.Schemes
{
    public class CustomJwtAuthenticationOptions : AuthenticationSchemeOptions
    {
        public string Authority { get; set; }
        public string ValidatePath { get; set; }
        public bool DisableServerCertificateValidation { get; set; }
    }
}