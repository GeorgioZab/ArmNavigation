using ArmNavigation.Domain.Enums;
using ArmNavigation.Domain.Models;
using ArnNavigation.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArmNavigation.Presentation.Controllers
{
    [ApiController]
    [Route("api/cars")]
    [Authorize]
    public sealed class CarsController : ControllerBase
    {
        private readonly ICarsService _service;
        private readonly UserRoleInfo _currentUser;

        public CarsController(ICarsService service, UserRoleInfo currentUser)
        {
            _service = service;
            _currentUser = currentUser;
        }

        private ActionResult<ResponseModel> Success() => Ok(new ResponseModel { Success = true });
        private ActionResult<ResponseModel> NotFoundResponse() => NotFound(new ResponseModel { Success = false, Error = "Not found" });
        private ActionResult<ResponseModel<T>> NotFoundResponse<T>() =>
            NotFound(new ResponseModel<T> { Success = false, Error = "Not found" });
        private ActionResult<ResponseModel<T>> OkData<T>(T data) => Ok(new ResponseModel<T> { Success = true, Data = data });
        private ActionResult<ResponseModel> Error(string message) => BadRequest(new ResponseModel { Success = false, Error = message });

        [HttpGet]
        public async Task<ActionResult<ResponseModel<IEnumerable<Car>>>> List([FromQuery] Guid? orgId, CancellationToken token)
        {
            var result = await _service.ListAsync(_currentUser.Role, _currentUser.MedInstitutionId, orgId, token);
            return OkData(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ResponseModel<Car>>> Get(Guid id, CancellationToken token)
        {
            var entity = await _service.GetAsync(id, _currentUser.Role, _currentUser.MedInstitutionId, token);
            return entity is null
                ? NotFoundResponse<Car>()
                : OkData(entity);
        }

        [HttpPost]
        public async Task<ActionResult<ResponseModel<Guid>>> Create([FromBody] CreateCarRequest request, CancellationToken token)
        {
            var id = await _service.CreateAsync(
                request.RegNum,
                request.MedInstitutionId,
                request.GpsTracker,
                _currentUser.Role,
                _currentUser.MedInstitutionId,
                token);

            return OkData(id);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ResponseModel>> Update(Guid id, [FromBody] UpdateCarRequest request, CancellationToken token)
        {
            var success = await _service.UpdateAsync(
                id,
                request.RegNum,
                request.MedInstitutionId,
                request.GpsTracker,
                _currentUser.Role,
                _currentUser.MedInstitutionId,
                token);

            return (success != null) ? Success() : NotFoundResponse();
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ResponseModel>> Delete(Guid id, CancellationToken token)
        {
            var success = await _service.RemoveAsync(id, _currentUser.Role, _currentUser.MedInstitutionId, token);
            return (success != null) ? Success() : NotFoundResponse();
        }

        [HttpGet("get")]
        public async Task<ActionResult<ResponseModel<IEnumerable<Car>>>> GetCars([FromQuery] string query, [FromQuery] Guid? orgId, CancellationToken token)
        {
            var result = await _service.GetAsync(query, _currentUser.Role, _currentUser.MedInstitutionId, orgId, token);
            return OkData(result);
        }

        [HttpPost("bind-tracker/{id:guid}")]
        public async Task<ActionResult<ResponseModel>> BindTracker(Guid id, [FromBody] BindTrackerRequest request, CancellationToken token)
        {
            var success = await _service.BindTrackerAsync(id, request.Tracker, _currentUser.Role, _currentUser.MedInstitutionId, token);
            return (success != null) ? Success() : NotFoundResponse();
        }

        [HttpPost("unbind-tracker/{id:guid}")]
        public async Task<ActionResult<ResponseModel>> UnbindTracker(Guid id, CancellationToken token)
        {
            var success = await _service.UnbindTrackerAsync(id, _currentUser.Role, _currentUser.MedInstitutionId, token);
            return (success != null) ? Success() : NotFoundResponse();
        }
    }
}