using ArmNavigation.Domain.Models;

namespace ArnNavigation.Application.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByLoginAsync(string login, CancellationToken cancellationToken);
        Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<Guid> CreateAsync(User user, CancellationToken cancellationToken);
        Task<bool> UpdateAsync(User user, CancellationToken cancellationToken);
        Task<bool> SoftDeleteAsync(Guid id, CancellationToken cancellationToken);
        Task<IEnumerable<User>> GetAllByOrgAsync(Guid? medInstitutionId, CancellationToken cancellationToken);
    }
}



