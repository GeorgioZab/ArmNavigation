using ArmNavigation.Domain.Enums;
using ArmNavigation.Domain.Models;

namespace ArnNavigation.Application.Services
{
    public interface ICarsService
    {
        Task<IEnumerable<Car>> ListAsync(int requesterRole, Guid requesterOrgId, Guid? orgId, CancellationToken token);
        Task<Car?> GetAsync(Guid id, int requesterRole, Guid requesterOrgId, CancellationToken token);
        Task<Guid> CreateAsync(string regNum, Guid orgId, string? gpsTracker, int requesterRole, Guid requesterOrgId, CancellationToken token);
        Task<Car> UpdateAsync(Guid id, string regNum, Guid orgId, string? gpsTracker, int requesterRole, Guid requesterOrgId, CancellationToken token);
        Task<Car> RemoveAsync(Guid id, int requesterRole, Guid requesterOrgId, CancellationToken token);
        Task<IEnumerable<Car>> GetAsync(string query, int requesterRole, Guid requesterOrgId, Guid? orgId, CancellationToken token);
        Task<Car> BindTrackerAsync(Guid carId, string tracker, int requesterRole, Guid requesterOrgId, CancellationToken token);
        Task<Car> UnbindTrackerAsync(Guid carId, int requesterRole, Guid requesterOrgId, CancellationToken token);
    }
}






