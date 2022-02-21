using JwtDemoWebApp.Common.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtDemoWebApp.Controllers
{
    [Authorize(AuthenticationSchemes=AuthenticationSchemes.CookiesAuthenticationScheme)]
    public class DocumentsController : Controller
    {
        // GET
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
    }
}