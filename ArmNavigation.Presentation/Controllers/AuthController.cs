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

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] LoginRequest request, CancellationToken token)
        {
            var response = await _authService.LoginAsync(request.Login, request.Password, token);
            if (response is null) return Unauthorized();
            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<ActionResult<string>> Register([FromBody] RegisterRequest request, CancellationToken token)
        {
            try
            {
                var response = await _authService.RegisterAsync(request.Login, request.Password, token);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}