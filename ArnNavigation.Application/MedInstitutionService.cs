using ArnNavigation.Application.Repositories;
using ArmNavigation.Domain.Enums;
using ArmNavigation.Domain.Models;

namespace ArnNavigation.Application.Services
{
    public sealed class MedInstitutionService : IMedInstitutionService
    {
        private readonly IMedInstitutionRepository _repository;

        public MedInstitutionService(IMedInstitutionRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<MedInstitution>> ListAsync(string? nameFilter, Role requesterRole, CancellationToken cancellationToken)
        {
            return _repository.GetAllByNameAsync(nameFilter, cancellationToken);
        }

        public Task<MedInstitution?> GetAsync(Guid id, Role requesterRole, CancellationToken cancellationToken)
        {
            return _repository.GetByIdAsync(id, cancellationToken);
        }

        public async Task<Guid> CreateAsync(string name, Role requesterRole, CancellationToken cancellationToken)
        {
            EnsureSuperAdmin(requesterRole);
            var entity = new MedInstitution { MedInstitutionId = Guid.NewGuid(), Name = name, IsRemoved = false };
            return await _repository.CreateAsync(entity, cancellationToken);
        }

        public async Task<bool> UpdateAsync(Guid id, string name, Role requesterRole, CancellationToken cancellationToken)
        {
            EnsureSuperAdmin(requesterRole);
            var entity = new MedInstitution { MedInstitutionId = id, Name = name, IsRemoved = false };
            return await _repository.UpdateAsync(entity, cancellationToken);
        }

        public async Task<bool> RemoveAsync(Guid id, Role requesterRole, CancellationToken cancellationToken)
        {
            EnsureSuperAdmin(requesterRole);
            return await _repository.SoftDeleteAsync(id, cancellationToken);
        }

        private static void EnsureSuperAdmin(Role role)
        {
            if (role != Role.SuperAdmin)
            {
                throw new UnauthorizedAccessException("Only SuperAdmin allowed");
            }
        }
    }
}



