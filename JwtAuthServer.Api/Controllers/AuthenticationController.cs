using System.Threading.Tasks;
using JwtAuth.Common.Models;
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
            var requestModel = model.Adapt<JwtAuthServer.Authentication.Models.UserLoginRequest>();
            var response = await _userService.LoginUserAsync(requestModel);

            if (!response.Succeeded)
            {
                return BadRequest(new {message = "Username or password is incorrect"});
            }

            var clientResponse = response.Adapt<UserLoginResponse>();

            return Ok(clientResponse);
        }

        [HttpPost("validate")]
        public async Task<IActionResult> Validate(ValidateTokenRequest model)
        {
            var requestModel = model.Adapt<JwtAuthServer.Authentication.Models.ValidateTokenRequest>();
            var response = await _userService.ValidateTokenAsync(requestModel);

            if (!response.Succeeded)
            {
                return BadRequest(new {message = "User token is incorrect"});
            }

            var clientResponse = response.Adapt<ValidateTokenResponse>();
            return Ok(clientResponse);
        }

    }
}