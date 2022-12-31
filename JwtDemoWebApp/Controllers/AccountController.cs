using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using JwtAuth.Common.Models;
using JwtDemoWebApp.Common.Constants;
using JwtDemoWebApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JwtDemoWebApp.Controllers
{
    [Authorize(AuthenticationSchemes=AuthenticationSchemes.CookiesAuthenticationScheme)]
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Tokens()
        {
            var token = Request.Cookies[CookieNames.XAccessToken];
            var refreshToken = Request.Cookies[CookieNames.XRefreshToken];
            @ViewBag.Token = token;
            @ViewBag.RefreshToken = refreshToken;
            
            return View();
        }

        [HttpPost]
        [ActionName("Refresh")]
        public async Task<IActionResult> RefreshPostAsync()
        {
            var httpClient = _httpClientFactory.CreateClient(HttpClientNames.ServerApiClient);

            var userName = User.Claims.First(x => x.Type == ClaimTypes.Name).Value;
            var token = Request.Cookies[CookieNames.XAccessToken];
            var refreshToken = Request.Cookies[CookieNames.XRefreshToken];
            
            var request = new
            {
                UserName = userName,
                Token = token,
                RefreshToken = refreshToken
            };

            var response = await httpClient.PostAsJsonAsync("/auth/rotate", request, CancellationToken.None);
            
            return RedirectToAction("Tokens");
        }

        [HttpPost]
        [AllowAnonymous]
        [ActionName("Login")]
        public async Task<IActionResult> LoginPostAsync(LoginViewModel viewModel, string returnUrl)
        {
            UserLoginResponse userLoginResponse = null;
            try
            {
                var httpClient = _httpClientFactory.CreateClient(HttpClientNames.ServerApiClient);

                var request = new
                {
                    UserName = viewModel.UserName,
                    Password = viewModel.Password
                };

                var response = await httpClient.PostAsJsonAsync("/auth/authenticate", request, CancellationToken.None);

                if (response.IsSuccessStatusCode)
                {
                    userLoginResponse = await response.Content.ReadFromJsonAsync<UserLoginResponse>();
                }
            }
            catch (Exception)
            {
                // ignored
            }

            if (userLoginResponse != null)
            {
                // You can store tokens securely in HttpOnly cookies.
                // Actually, the local storage is the better option
                Response.Cookies.Append(
                    CookieNames.XAccessToken,
                    userLoginResponse.JwtToken, new CookieOptions
                    {
                        HttpOnly = true,
                        SameSite = SameSiteMode.Strict
                    });
                
                Response.Cookies.Append(
                    CookieNames.XRefreshToken,
                    userLoginResponse.RefreshToken, new CookieOptions
                    {
                        HttpOnly = true,
                        SameSite = SameSiteMode.Strict
                    });

                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(userLoginResponse.JwtToken);
                
                var claims = new List<Claim>
                {
                    new (ClaimTypes.Name, userLoginResponse.UserName)
                };

                foreach (var roleClaim in jwtToken.Claims.Where(c => c.Type == "role"))
                {
                    claims.Add(new Claim(ClaimTypes.Role, roleClaim.Value));
                }

                foreach (var jwtTokenClaim in jwtToken.Claims)
                {
                    claims.Add(
                        new (jwtTokenClaim.Type, jwtTokenClaim.Value));
                }

                var identity = new ClaimsIdentity(claims, AuthenticationSchemes.CookiesAuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(AuthenticationSchemes.CookiesAuthenticationScheme, new ClaimsPrincipal(principal));

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Error = "Please try again";
                return View("Login");
            }
        }

        [HttpPost]
        [ActionName("Logout")]
        public async Task<IActionResult> LogoutPostAsync(string returnUrl)
        {
            await HttpContext.SignOutAsync(AuthenticationSchemes.CookiesAuthenticationScheme);
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            };
        }
        
        public IActionResult Forbidden()
        {
            Response.StatusCode = 404;
            Response.ContentType = "text/html";

            return View();
        }
    }
}