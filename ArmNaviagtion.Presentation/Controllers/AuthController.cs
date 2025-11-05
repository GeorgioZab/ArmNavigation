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
        public async Task<ActionResult<string>> Login([FromBody] LoginRequest request, CancellationToken ct)
        {
            var token = await _authService.LoginAsync(request.Login, request.Password, ct);
            if (token is null) return Unauthorized();
            return Ok(token);
        }
    }
}



