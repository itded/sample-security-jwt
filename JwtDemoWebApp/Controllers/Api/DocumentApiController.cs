using System;
using JwtDemoWebApp.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtDemoWebApp.Controllers.Api
{
    [ApiController]
    [ApiVersion("1.0")]
    // [Route("api/v{version:apiVersion}/documents")]
    [Route("api/documents")]
    [Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme)]
    public class DocumentApiController : ControllerBase
    {
        private readonly IDocumentService _documentService;

        public DocumentApiController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        [HttpGet("GetAllDocumentsByRole")]
        public void GetAllDocumentsByRole()
        {
            throw new NotImplementedException();
            // // TODO: read token, validate, extract the role claim
            // User.FindFirst(ClaimTypes.NameIdentifier)
            // _documentService.GetAllDocumentsByRole();
        }
    }
}