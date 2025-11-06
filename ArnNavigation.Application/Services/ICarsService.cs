using ArmNavigation.Domain.Enums;
using ArmNavigation.Domain.Models;

namespace ArnNavigation.Application.Services
{
    public interface ICarsService
    {
        Task<IEnumerable<Car>> ListAsync(int requesterRole, Guid requesterOrgId, Guid? orgId, CancellationToken ct);
        Task<Car?> GetAsync(Guid id, int requesterRole, Guid requesterOrgId, CancellationToken ct);
        Task<Guid> CreateAsync(string regNum, Guid orgId, string? gpsTracker, int requesterRole, Guid requesterOrgId, CancellationToken ct);
        Task<bool> UpdateAsync(Guid id, string regNum, Guid orgId, string? gpsTracker, int requesterRole, Guid requesterOrgId, CancellationToken ct);
        Task<bool> RemoveAsync(Guid id, int requesterRole, Guid requesterOrgId, CancellationToken ct);
        Task<IEnumerable<Car>> GetAsync(string query, int requesterRole, Guid requesterOrgId, Guid? orgId, CancellationToken ct);
        Task<bool> BindTrackerAsync(Guid carId, string tracker, int requesterRole, Guid requesterOrgId, CancellationToken ct);
        Task<bool> UnbindTrackerAsync(Guid carId, int requesterRole, Guid requesterOrgId, CancellationToken ct);
    }
}





