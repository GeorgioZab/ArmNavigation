using ArmNavigation.Domain.Enums;
using ArmNavigation.Domain.Models;

namespace ArnNavigation.Application.Services
{
    public interface IUsersService
    {
        Task<IEnumerable<User>> ListAsync(Role requesterRole, Guid requesterOrgId, Guid? orgId, CancellationToken ct);
        Task<User?> GetAsync(Guid id, Role requesterRole, Guid requesterOrgId, CancellationToken ct);
        Task<Guid> CreateAsync(string login, string passwordPlain, Role role, Guid orgId, Role requesterRole, Guid requesterOrgId, CancellationToken ct);
        Task<bool> UpdateAsync(Guid id, string login, string? passwordPlain, Role role, Guid orgId, Role requesterRole, Guid requesterOrgId, CancellationToken ct);
        Task<bool> RemoveAsync(Guid id, Role requesterRole, Guid requesterOrgId, CancellationToken ct);
    }
}



