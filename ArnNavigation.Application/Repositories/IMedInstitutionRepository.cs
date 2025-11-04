using ArmNavigation.Domain.Models;

namespace ArnNavigation.Application.Repositories
{
    public interface IMedInstitutionRepository
    {
        Task<IEnumerable<MedInstitution>> GetAllAsync(CancellationToken cancellationToken);
        Task<IEnumerable<MedInstitution>> GetAllByNameAsync(string? nameFilter, CancellationToken cancellationToken);
        Task<MedInstitution?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<Guid> CreateAsync(MedInstitution medInstitution, CancellationToken cancellationToken);
        Task<bool> UpdateAsync(MedInstitution medInstitution, CancellationToken cancellationToken);
        Task<bool> SoftDeleteAsync(Guid id, CancellationToken cancellationToken);
    }
}



