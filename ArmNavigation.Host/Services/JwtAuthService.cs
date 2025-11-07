using ArnNavigation.Application.Repositories;
using ArnNavigation.Application.Services;
using ArmNavigation.Domain.Models;

namespace ArmNavigation.Services
{
    public sealed class JwtAuthService : IAuthService
    {
        private readonly IUserRepository _users;
        private readonly JwtTokenService _jwtTokenService;
        private readonly IPasswordHasher _passwordHasher;

        public JwtAuthService(IUserRepository users, JwtTokenService jwtTokenService, IPasswordHasher passwordHasher)
        {
            _users = users;
            _jwtTokenService = jwtTokenService;
            _passwordHasher = passwordHasher;
        }

        public async Task<string?> LoginAsync(string login, string password, CancellationToken cancellationToken)
        {
            var user = await _users.GetByLoginAsync(login, cancellationToken);
            if (user is null) return null;
            if (!_passwordHasher.Verify(password, user.PasswordHash)) return null;

            return _jwtTokenService.GenerateToken(user.UserId, user.Login, user.Role, user.MedInstitutionId);
        }

        public async Task<string> RegisterAsync(string login, string password, CancellationToken cancellationToken)
        {
            var existingUser = await _users.GetByLoginAsync(login, cancellationToken);
            if (existingUser != null)
            {
                throw new InvalidOperationException("User with this login already exists");
            }

            var passwordHash = _passwordHasher.Hash(password);

            var user = new User
            {
                UserId = Guid.NewGuid(),
                Login = login,
                PasswordHash = passwordHash,
                MedInstitutionId = Guid.Empty,
                Role = 2,
                IsRemoved = false
            };

            await _users.CreateAsync(user, cancellationToken);

            return _jwtTokenService.GenerateToken(user.UserId, user.Login, user.Role, user.MedInstitutionId);
        }
    }
}