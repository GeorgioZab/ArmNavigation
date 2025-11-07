using ArnNavigation.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace ArmNaviagtion.Presentation.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public sealed class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        public sealed record LoginRequest(string Login, string Password);

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] LoginRequest request, CancellationToken token)
        {
            var response = await _authService.LoginAsync(request.Login, request.Password, token);
            if (response is null) return Unauthorized();
            return Ok(response);
        }
    }
}



