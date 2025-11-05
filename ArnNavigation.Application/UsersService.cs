using ArnNavigation.Application.Repositories;
using ArmNavigation.Domain.Enums;
using ArmNavigation.Domain.Models;
using ArnNavigation.Application.Services;

namespace ArnNavigation.Application.Services
{
    public sealed class UsersService : IUsersService
    {
        private readonly IUserRepository _users;
        private readonly IPasswordHasher _passwordHasher;

        public UsersService(IUserRepository users, IPasswordHasher passwordHasher)
        {
            _users = users;
            _passwordHasher = passwordHasher;
        }

        public Task<IEnumerable<User>> ListAsync(int requesterRole, Guid requesterOrgId, Guid? orgId, CancellationToken ct)
        {
            var scopeOrg = requesterRole == (int)Role.SuperAdmin ? orgId : requesterOrgId;
            return _users.GetAllByOrgAsync(scopeOrg, ct);
        }

        public async Task<User?> GetAsync(Guid id, int requesterRole, Guid requesterOrgId, CancellationToken ct)
        {
            var u = await _users.GetByIdAsync(id, ct);
            if (u is null) return null;
            if (requesterRole != (int)Role.SuperAdmin && u.MedInstitutionId != requesterOrgId) return null;
            return u;
        }

        public async Task<Guid> CreateAsync(string login, string passwordPlain, int role, Guid orgId, int requesterRole, Guid requesterOrgId, CancellationToken ct)
        {
            if (requesterRole != (int)Role.SuperAdmin && requesterOrgId != orgId) throw new UnauthorizedAccessException();
            var hashed = _passwordHasher.Hash(passwordPlain);
            var entity = new User
            {
                UserId = Guid.NewGuid(),
                Login = login,
                PasswordHash = hashed,
                MedInstitutionId = orgId,
                Role = role,
                IsRemoved = false
            };
            return await _users.CreateAsync(entity, ct);
        }

        public async Task<bool> UpdateAsync(Guid id, string login, string? passwordPlain, int role, Guid orgId, int requesterRole, Guid requesterOrgId, CancellationToken ct)
        {
            var existing = await _users.GetByIdAsync(id, ct);
            if (existing is null) return false;
            if (requesterRole != (int)Role.SuperAdmin && existing.MedInstitutionId != requesterOrgId) throw new UnauthorizedAccessException();

            var hashed = passwordPlain is null ? existing.PasswordHash : _passwordHasher.Hash(passwordPlain);
            var updated = new User
            {
                UserId = id,
                Login = login,
                PasswordHash = hashed,
                MedInstitutionId = orgId,
                Role = role,
                IsRemoved = false
            };
            return await _users.UpdateAsync(updated, ct);
        }

        public async Task<bool> RemoveAsync(Guid id, int requesterRole, Guid requesterOrgId, CancellationToken ct)
        {
            var existing = await _users.GetByIdAsync(id, ct);
            if (existing is null) return false;
            if (requesterRole != (int)Role.SuperAdmin && existing.MedInstitutionId != requesterOrgId) throw new UnauthorizedAccessException();
            return await _users.SoftDeleteAsync(id, ct);
        }
    }
}


