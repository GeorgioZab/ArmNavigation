using ArnNavigation.Application.Services;
using ArmNavigation.Domain.Enums;
using ArmNavigation.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ArmNaviagtion.Presentation.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public sealed class UsersController : ControllerBase
    {
        private readonly IUsersService _service;

        public UsersController(IUsersService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> List([FromQuery] Guid? orgId, CancellationToken token)
        {
            (int role, Guid org) = GetContext(User);
            var result = await _service.ListAsync(role, org, orgId, token);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<User?>> Get(Guid id, CancellationToken token)
        {
            (int role, Guid org) = GetContext(User);
            var user = await _service.GetAsync(id, role, org, token);
            if (user is null) return NotFound();
            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateUserRequest request, CancellationToken token)
        {
            (int role, Guid org) = GetContext(User);
            var id = await _service.CreateAsync(request.Login, request.Password, request.Role, request.MedInstitutionId, role, org, token);
            return CreatedAtAction(nameof(Get), new { id }, id);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult> Update(Guid id, [FromBody] UpdateUserRequest request, CancellationToken token)
        {
            (int role, Guid org) = GetContext(User);
            var ok = await _service.UpdateAsync(id, request.Login, request.Password, request.Role, request.MedInstitutionId, role, org, token);
            if (ok == null) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> Delete(Guid id, CancellationToken token)
        {
            (int role, Guid org) = GetContext(User);
            var ok = await _service.RemoveAsync(id, role, org, token);
            if (ok == null) return NotFound();
            return NoContent();
        }

        private static (int role, Guid org) GetContext(ClaimsPrincipal user)
        {
            var roleClaim = user.FindFirst(ClaimTypes.Role)?.Value;
            var orgClaim = user.FindFirst("org")?.Value;
            var role = int.TryParse(roleClaim, out var r) ? r : (int)Role.Dispatcher;
            var org = Guid.TryParse(orgClaim, out var g) ? g : Guid.Empty;
            return (role, org);
        }
    }
}



