using System;
using System.Linq;
using System.Security.Claims;
using JwtDemoWebApp.Common.Enums;
using JwtDemoWebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtDemoWebApp.Controllers
{
    public class DocumentsController : Controller
    {
        private readonly IDocumentService _documentService;

        public DocumentsController(IDocumentService documentService)
        {
            _documentService = documentService;
        }
        
        // GET
        [Authorize]
        public IActionResult Index()
        {
            var roles = User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => Map(c.Value))
                .ToArray();

            if (!roles.Any())
            {
                return View();
            }

            var documents = _documentService.GetAllDocumentsByRoles(roles);
            return View(documents);
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