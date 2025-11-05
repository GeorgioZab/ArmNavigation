using ArmNavigation.Domain.Enums;
using ArmNavigation.Domain.Models;

namespace ArnNavigation.Application.Services
{
    public interface IMedInstitutionService
    {
        Task<IEnumerable<MedInstitution>> ListAsync(string? nameFilter, int requesterRole, CancellationToken cancellationToken);
        Task<MedInstitution?> GetAsync(Guid id, int requesterRole, CancellationToken cancellationToken);
        Task<Guid> CreateAsync(string name, int requesterRole, CancellationToken cancellationToken);
        Task<bool> UpdateAsync(Guid id, string name, int requesterRole, CancellationToken cancellationToken);
        Task<bool> RemoveAsync(Guid id, int requesterRole, CancellationToken cancellationToken);
    }
}



