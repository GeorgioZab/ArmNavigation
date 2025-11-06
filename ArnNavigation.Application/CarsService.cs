using ArnNavigation.Application.Repositories;
using ArmNavigation.Domain.Enums;
using ArmNavigation.Domain.Models;

namespace ArnNavigation.Application.Services
{
    public sealed class CarsService : ICarsService
    {
        private readonly ICarRepository _cars;

        public CarsService(ICarRepository cars)
        {
            _cars = cars;
        }

        public Task<IEnumerable<Car>> ListAsync(int requesterRole, Guid requesterOrgId, Guid? orgId, CancellationToken ct)
        {
            var scopeOrg = requesterRole == (int)Role.SuperAdmin ? orgId : requesterOrgId;
            return _cars.GetAllByOrgAsync(scopeOrg, ct);
        }

        public async Task<Car?> GetAsync(Guid id, int requesterRole, Guid requesterOrgId, CancellationToken ct)
        {
            var car = await _cars.GetByIdAsync(id, ct);
            if (car is null) return null;
            if (requesterRole != (int)Role.SuperAdmin && car.MedInstitutionId != requesterOrgId) return null;
            return car;
        }

        public async Task<Guid> CreateAsync(string regNum, Guid orgId, string? gpsTracker, int requesterRole, Guid requesterOrgId, CancellationToken ct)
        {
            if (requesterRole != (int)Role.SuperAdmin && requesterOrgId != orgId) throw new UnauthorizedAccessException();
            var entity = new Car
            {
                CarId = Guid.NewGuid(),
                RegNum = regNum,
                MedInstitutionId = orgId,
                GpsTracker = gpsTracker,
                IsRemoved = false
            };
            return await _cars.CreateAsync(entity, ct);
        }

        public async Task<bool> UpdateAsync(Guid id, string regNum, Guid orgId, string? gpsTracker, int requesterRole, Guid requesterOrgId, CancellationToken ct)
        {
            var existing = await _cars.GetByIdAsync(id, ct);
            if (existing is null) return false;
            if (requesterRole != (int)Role.SuperAdmin && existing.MedInstitutionId != requesterOrgId) throw new UnauthorizedAccessException();

            var updated = new Car
            {
                CarId = id,
                RegNum = regNum,
                MedInstitutionId = orgId,
                GpsTracker = gpsTracker,
                IsRemoved = false
            };
            return await _cars.UpdateAsync(updated, ct);
        }

        public async Task<bool> RemoveAsync(Guid id, int requesterRole, Guid requesterOrgId, CancellationToken ct)
        {
            var existing = await _cars.GetByIdAsync(id, ct);
            if (existing is null) return false;
            if (requesterRole != (int)Role.SuperAdmin && existing.MedInstitutionId != requesterOrgId) throw new UnauthorizedAccessException();
            return await _cars.SoftDeleteAsync(id, ct);
        }

        {
            var scopeOrg = requesterRole == (int)Role.SuperAdmin ? orgId : requesterOrgId;
            return _cars.GetAsync(query, scopeOrg, ct);
        }

        public async Task<bool> BindTrackerAsync(Guid carId, string tracker, int requesterRole, Guid requesterOrgId, CancellationToken ct)
        {
            var existing = await _cars.GetByIdAsync(carId, ct);
            if (existing is null) return false;
            if (requesterRole != (int)Role.SuperAdmin && existing.MedInstitutionId != requesterOrgId) throw new UnauthorizedAccessException();
            return await _cars.BindTrackerAsync(carId, tracker, ct);
        }

        public async Task<bool> UnbindTrackerAsync(Guid carId, int requesterRole, Guid requesterOrgId, CancellationToken ct)
        {
            var existing = await _cars.GetByIdAsync(carId, ct);
            if (existing is null) return false;
            if (requesterRole != (int)Role.SuperAdmin && existing.MedInstitutionId != requesterOrgId) throw new UnauthorizedAccessException();
            return await _cars.UnbindTrackerAsync(carId, ct);
        }
    }
}





