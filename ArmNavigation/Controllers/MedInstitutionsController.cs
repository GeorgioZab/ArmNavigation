using ArnNavigation.Application.Services;
using ArmNavigation.Domain.Enums;
using ArmNavigation.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ArmNavigation.Controllers
{
    [ApiController]
    [Route("api/med-institutions")]
    public sealed class MedInstitutionsController : ControllerBase
    {
        private readonly IMedInstitutionService _service;

        public MedInstitutionsController(IMedInstitutionService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MedInstitution>>> List([FromQuery] string? name, CancellationToken ct)
        {
            var role = GetRoleFromUser(User);
            var result = await _service.ListAsync(name, role, ct);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<MedInstitution?>> Get(Guid id, CancellationToken ct)
        {
            var role = GetRoleFromUser(User);
            var entity = await _service.GetAsync(id, role, ct);
            if (entity is null) return NotFound();
            return Ok(entity);
        }

        public sealed record CreateMedInstitutionRequest(string Name);
        public sealed record UpdateMedInstitutionRequest(string Name);

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateMedInstitutionRequest request, CancellationToken ct)
        {
            var role = GetRoleFromUser(User);
            var id = await _service.CreateAsync(request.Name, role, ct);
            return CreatedAtAction(nameof(Get), new { id }, id);
        }

        [HttpPut("{id:guid}")]
        [Authorize]
        public async Task<ActionResult> Update(Guid id, [FromBody] UpdateMedInstitutionRequest request, CancellationToken ct)
        {
            var role = GetRoleFromUser(User);
            var ok = await _service.UpdateAsync(id, request.Name, role, ct);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [Authorize]
        public async Task<ActionResult> Delete(Guid id, CancellationToken ct)
        {
            var role = GetRoleFromUser(User);
            var ok = await _service.RemoveAsync(id, role, ct);
            if (!ok) return NotFound();
            return NoContent();
        }

        private static Role GetRoleFromUser(ClaimsPrincipal user)
        {
            var roleClaim = user.FindFirst(ClaimTypes.Role)?.Value;
            return roleClaim switch
            {
                nameof(Role.SuperAdmin) => Role.SuperAdmin,
                nameof(Role.OrgAdmin) => Role.OrgAdmin,
                nameof(Role.Dispatcher) => Role.Dispatcher,
                _ => Role.Dispatcher
            };
        }
    }
}



