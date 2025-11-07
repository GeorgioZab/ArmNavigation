using ArmNavigation.Domain.Enums;
using ArmNavigation.Domain.Models;

namespace ArnNavigation.Application.Services
{
    public interface IUsersService
    {
        Task<IEnumerable<User>> ListAsync(int requesterRole, Guid requesterOrgId, Guid? orgId, CancellationToken token);
        Task<User?> GetAsync(Guid id, int requesterRole, Guid requesterOrgId, CancellationToken token);
        Task<Guid> CreateAsync(string login, string passwordPlain, int role, Guid orgId, int requesterRole, Guid requesterOrgId, CancellationToken token);
        Task<User> UpdateAsync(Guid id, string login, string? passwordPlain, int role, Guid orgId, int requesterRole, Guid requesterOrgId, CancellationToken token);
        Task<User> RemoveAsync(Guid id, int requesterRole, Guid requesterOrgId, CancellationToken token);
    }
}



