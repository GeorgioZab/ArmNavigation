using ArnNavigation.Application.Services;
using ArmNavigation.Domain.Enums;
using ArmNavigation.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ArmNavigation.Controllers
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
        public async Task<ActionResult<IEnumerable<User>>> List([FromQuery] Guid? orgId, CancellationToken ct)
        {
            var (role, org) = GetContext(User);
            var result = await _service.ListAsync(role, org, orgId, ct);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<User?>> Get(Guid id, CancellationToken ct)
        {
            var (role, org) = GetContext(User);
            var user = await _service.GetAsync(id, role, org, ct);
            if (user is null) return NotFound();
            return Ok(user);
        }

        public sealed record CreateUserRequest(string Login, string Password, Role Role, Guid MedInstitutionId);
        public sealed record UpdateUserRequest(string Login, string? Password, Role Role, Guid MedInstitutionId);

        [HttpPost]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateUserRequest request, CancellationToken ct)
        {
            var (role, org) = GetContext(User);
            var id = await _service.CreateAsync(request.Login, request.Password, request.Role, request.MedInstitutionId, role, org, ct);
            return CreatedAtAction(nameof(Get), new { id }, id);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult> Update(Guid id, [FromBody] UpdateUserRequest request, CancellationToken ct)
        {
            var (role, org) = GetContext(User);
            var ok = await _service.UpdateAsync(id, request.Login, request.Password, request.Role, request.MedInstitutionId, role, org, ct);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> Delete(Guid id, CancellationToken ct)
        {
            var (role, org) = GetContext(User);
            var ok = await _service.RemoveAsync(id, role, org, ct);
            if (!ok) return NotFound();
            return NoContent();
        }

        private static (Role role, Guid org) GetContext(ClaimsPrincipal user)
        {
            var roleClaim = user.FindFirst(ClaimTypes.Role)?.Value;
            var orgClaim = user.FindFirst("org")?.Value;
            var role = roleClaim switch
            {
                nameof(Role.SuperAdmin) => Role.SuperAdmin,
                nameof(Role.OrgAdmin) => Role.OrgAdmin,
                nameof(Role.Dispatcher) => Role.Dispatcher,
                _ => Role.Dispatcher
            };
            var org = Guid.TryParse(orgClaim, out var g) ? g : Guid.Empty;
            return (role, org);
        }
    }
}



