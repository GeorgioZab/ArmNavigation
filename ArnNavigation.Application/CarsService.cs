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

        public Task<IEnumerable<Car>> ListAsync(int requesterRole, Guid requesterOrgId, Guid? orgId, CancellationToken token)
        {
            var scopeOrg = requesterRole == (int)Role.SuperAdmin ? orgId : requesterOrgId;
            return _cars.GetAllByOrgAsync(scopeOrg, token);
        }

        public async Task<Car?> GetAsync(Guid id, int requesterRole, Guid requesterOrgId, CancellationToken token)
        {
            var car = await _cars.GetByIdAsync(id, token);
            if (car is null) return null;
            if (requesterRole != (int)Role.SuperAdmin && car.MedInstitutionId != requesterOrgId) return null;
            return car;
        }

        public async Task<Guid> CreateAsync(string regNum, Guid orgId, string? gpsTracker, int requesterRole, Guid requesterOrgId, CancellationToken token)
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
            return await _cars.CreateAsync(entity, token);
        }

        public async Task<Car> UpdateAsync(Guid id, string regNum, Guid orgId, string? gpsTracker, int requesterRole, Guid requesterOrgId, CancellationToken token)
        {
            var existing = await _cars.GetByIdAsync(id, token);
            if (existing is null) return null;
            if (requesterRole != (int)Role.SuperAdmin && existing.MedInstitutionId != requesterOrgId) throw new UnauthorizedAccessException();

            var updated = new Car
            {
                CarId = id,
                RegNum = regNum,
                MedInstitutionId = orgId,
                GpsTracker = gpsTracker,
                IsRemoved = false
            };
            return await _cars.UpdateAsync(updated, token);
        }

        public async Task<Car> RemoveAsync(Guid id, int requesterRole, Guid requesterOrgId, CancellationToken token)
        {
            var existing = await _cars.GetByIdAsync(id, token);
            if (existing is null) return null;
            if (requesterRole != (int)Role.SuperAdmin && existing.MedInstitutionId != requesterOrgId) throw new UnauthorizedAccessException();
            return await _cars.SoftDeleteAsync(id, token);
        }

        public Task<IEnumerable<Car>> GetAsync(string query, int requesterRole, Guid requesterOrgId, Guid? orgId, CancellationToken token)
        {
            var scopeOrg = requesterRole == (int)Role.SuperAdmin ? orgId : requesterOrgId;
            return _cars.GetAsync(query, scopeOrg, token);
        }

        public async Task<Car> BindTrackerAsync(Guid carId, string tracker, int requesterRole, Guid requesterOrgId, CancellationToken token)
        {
            var existing = await _cars.GetByIdAsync(carId, token);
            if (existing is null) return null;
            if (requesterRole != (int)Role.SuperAdmin && existing.MedInstitutionId != requesterOrgId) throw new UnauthorizedAccessException();
            return await _cars.BindTrackerAsync(carId, tracker, token);
        }

        public async Task<Car> UnbindTrackerAsync(Guid carId, int requesterRole, Guid requesterOrgId, CancellationToken token)
        {
            var existing = await _cars.GetByIdAsync(carId, token);
            if (existing is null) return null;
            if (requesterRole != (int)Role.SuperAdmin && existing.MedInstitutionId != requesterOrgId) throw new UnauthorizedAccessException();
            return await _cars.UnbindTrackerAsync(carId, token);
        }
    }
}






