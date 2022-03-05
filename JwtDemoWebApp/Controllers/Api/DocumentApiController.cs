using System;
using System.Linq;
using System.Security.Claims;
using JwtDemoWebApp.Common.Constants;
using JwtDemoWebApp.Common.Enums;
using JwtDemoWebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtDemoWebApp.Controllers.Api
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/documents")]
    [Route("api/documents")]
    [Authorize(AuthenticationSchemes=AuthenticationSchemes.JwtAuthenticationScheme)]
    public class DocumentApiController : ControllerBase
    {
        private readonly IDocumentService _documentService;

        public DocumentApiController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        [HttpGet("getByRoles")]
        public IActionResult GetAllDocumentsByRoles()
        {
            var roles = User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => Map(c.Value))
                .ToArray();

            if (!roles.Any())
            {
                return new EmptyResult();
            }

            var documents = _documentService.GetAllDocumentsByRoles(roles);
            return new JsonResult(documents);
        }

        private UserRoleEnum Map(string roleName)
        {
            if (roleName.ToUpperInvariant().Equals("R_TESTER"))
            {
                return UserRoleEnum.Tester;
            }

            if (roleName.ToUpperInvariant().Equals("R_USER"))
            {
                return UserRoleEnum.User;
            }

            throw new ArgumentOutOfRangeException(nameof(roleName));
        }
    }
}