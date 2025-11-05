using ArnNavigation.Application.Services;
using ArmNavigation.Domain.Enums;
using ArmNavigation.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ArmNaviagtion.Presentation.Controllers
{
    [ApiController]
    [Route("api/cars")]
    [Authorize]
    public sealed class CarsController : ControllerBase
    {
        private readonly ICarsService _service;

        public CarsController(ICarsService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Car>>> List([FromQuery] Guid? orgId, CancellationToken ct)
        {
            var (role, org) = GetContext(User);
            var result = await _service.ListAsync(role, org, orgId, ct);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Car?>> Get(Guid id, CancellationToken ct)
        {
            var (role, org) = GetContext(User);
            var entity = await _service.GetAsync(id, role, org, ct);
            if (entity is null) return NotFound();
            return Ok(entity);
        }

        public sealed record CreateCarRequest(string RegNum, Guid MedInstitutionId, string? GpsTracker);
        public sealed record UpdateCarRequest(string RegNum, Guid MedInstitutionId, string? GpsTracker);

        [HttpPost]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateCarRequest request, CancellationToken ct)
        {
            var (role, org) = GetContext(User);
            var id = await _service.CreateAsync(request.RegNum, request.MedInstitutionId, request.GpsTracker, role, org, ct);
            return CreatedAtAction(nameof(Get), new { id }, id);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult> Update(Guid id, [FromBody] UpdateCarRequest request, CancellationToken ct)
        {
            var (role, org) = GetContext(User);
            var ok = await _service.UpdateAsync(id, request.RegNum, request.MedInstitutionId, request.GpsTracker, role, org, ct);
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

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Car>>> Search([FromQuery] string q, [FromQuery] Guid? orgId, CancellationToken ct)
        {
            var (role, org) = GetContext(User);
            var result = await _service.SearchAsync(q, role, org, orgId, ct);
            return Ok(result);
        }

        public sealed record BindTrackerRequest(string Tracker);

        [HttpPost("{id:guid}/bind-tracker")]
        public async Task<ActionResult> BindTracker(Guid id, [FromBody] BindTrackerRequest request, CancellationToken ct)
        {
            var (role, org) = GetContext(User);
            var ok = await _service.BindTrackerAsync(id, request.Tracker, role, org, ct);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpPost("{id:guid}/unbind-tracker")]
        public async Task<ActionResult> UnbindTracker(Guid id, CancellationToken ct)
        {
            var (role, org) = GetContext(User);
            var ok = await _service.UnbindTrackerAsync(id, role, org, ct);
            if (!ok) return NotFound();
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



