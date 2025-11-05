using ArmNavigation.Domain.Enums;
using ArmNavigation.Domain.Models;

namespace ArnNavigation.Application.Services
{
    public interface IUsersService
    {
        Task<IEnumerable<User>> ListAsync(int requesterRole, Guid requesterOrgId, Guid? orgId, CancellationToken ct);
        Task<User?> GetAsync(Guid id, int requesterRole, Guid requesterOrgId, CancellationToken ct);
        Task<Guid> CreateAsync(string login, string passwordPlain, int role, Guid orgId, int requesterRole, Guid requesterOrgId, CancellationToken ct);
        Task<bool> UpdateAsync(Guid id, string login, string? passwordPlain, int role, Guid orgId, int requesterRole, Guid requesterOrgId, CancellationToken ct);
        Task<bool> RemoveAsync(Guid id, int requesterRole, Guid requesterOrgId, CancellationToken ct);
    }
}



