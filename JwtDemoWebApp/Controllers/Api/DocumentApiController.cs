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
    // [Route("api/v{version:apiVersion}/documents")]
    [Route("api/documents")]
    [Authorize(AuthenticationSchemes=AuthenticationSchemes.JwtAuthenticationScheme)]
    public class DocumentApiController : ControllerBase
    {
        private readonly IDocumentService _documentService;

        public DocumentApiController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        [HttpGet("GetAllDocumentsByRole")]
        public IActionResult GetAllDocumentsByRole()
        {
            // TODO: extract roles
            var role = User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .FirstOrDefault();

            if (role == null)
            {
                return new EmptyResult();
            }

            var documents = _documentService.GetAllDocumentsByRole(UserRoleEnum.Tester);
            return new JsonResult(documents);
        }
    }
}