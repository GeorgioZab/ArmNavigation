using ArnNavigation.Application.Services;
using ArmNavigation.Domain.Enums;
using ArmNavigation.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ArmNaviagtion.Presentation.Controllers
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
        [Authorize]
        public async Task<ActionResult<IEnumerable<MedInstitution>>> List([FromQuery] string? name, CancellationToken token)
        {
            int role = GetRoleFromUser(User);
            var result = await _service.ListAsync(name, role, token);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [Authorize]
        public async Task<ActionResult<MedInstitution?>> Get(Guid id, CancellationToken token)
        {
            int role = GetRoleFromUser(User);
            var entity = await _service.GetAsync(id, role, token);
            if (entity is null) return NotFound();
            return Ok(entity);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateMedInstitutionRequest request, CancellationToken token)
        {
            int role = GetRoleFromUser(User);
            var id = await _service.CreateAsync(request.Name, role, token);
            return CreatedAtAction(nameof(Get), new { id }, id);
        }

        [HttpPut("{id:guid}")]
        [Authorize]
        public async Task<ActionResult> Update(Guid id, [FromBody] UpdateMedInstitutionRequest request, CancellationToken token)
        {
            int role = GetRoleFromUser(User);
            var ok = await _service.UpdateAsync(id, request.Name, role, token);
            if (ok == null) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [Authorize]
        public async Task<ActionResult> Delete(Guid id, CancellationToken token)
        {
            int role = GetRoleFromUser(User);
            var ok = await _service.RemoveAsync(id, role, token);
            if (ok == null) return NotFound();
            return NoContent();
        }

        private static int GetRoleFromUser(ClaimsPrincipal user)
        {
            var roleClaim = user.FindFirst(ClaimTypes.Role)?.Value;
            return int.TryParse(roleClaim, out var r) ? r : (int)Role.Dispatcher;
        }
    }
}



