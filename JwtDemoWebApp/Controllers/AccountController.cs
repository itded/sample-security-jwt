using Microsoft.AspNetCore.Mvc;

namespace JwtDemoWebApp.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Forbidden()
        {
            Response.StatusCode = 404;
            Response.ContentType = "text/html";

            return View();
        }
    }
}