using ArmNavigation.Domain.Models;

namespace ArnNavigation.Application.Repositories
{
    public interface ICarRepository
    {
        Task<IEnumerable<Car>> GetAllByOrgAsync(Guid? medInstitutionId, CancellationToken ct);
        Task<Car?> GetByIdAsync(Guid id, CancellationToken ct);
        Task<Guid> CreateAsync(Car car, CancellationToken ct);
        Task<bool> UpdateAsync(Car car, CancellationToken ct);
        Task<bool> SoftDeleteAsync(Guid id, CancellationToken ct);
        Task<IEnumerable<Car>> SearchAsync(string query, Guid? medInstitutionId, CancellationToken ct);
        Task<bool> BindTrackerAsync(Guid carId, string tracker, CancellationToken ct);
        Task<bool> UnbindTrackerAsync(Guid carId, CancellationToken ct);
    }
}

