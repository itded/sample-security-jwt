using System.Threading.Tasks;
using JwtAuthServer.Authentication.Models;
using JwtAuthServer.Authentication.Services;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuthServer.Api.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAppUserService _userService;

        public AuthenticationController(IAppUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate(UserLoginRequest model)
        {
            var response = await _userService.LoginUserAsync(model);

            if (!response.Succeeded)
            {
                return BadRequest(new {message = "Username or password is incorrect"});
            }

            var clientResponse = response.Adapt<JwtAuth.Common.Models.UserLoginResponse>();

            return Ok(clientResponse);
        }

        [HttpPost("validate")]
        public async Task<IActionResult> Validate(ValidateTokenRequest model)
        {
            var response = await _userService.ValidateTokenAsync(model);

            if (!response.Succeeded)
            {
                return BadRequest(new {message = "User token is incorrect"});
            }

            return Ok(response);
        }

    }
}