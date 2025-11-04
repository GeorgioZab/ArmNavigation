using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ArnNavigation.Application.Repositories;
using ArnNavigation.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ArmNavigation.Services
{
    public sealed class JwtAuthService : IAuthService
    {
        private readonly IUserRepository _users;
        private readonly IConfiguration _configuration;
        private readonly IPasswordHasher _passwordHasher;

        public JwtAuthService(IUserRepository users, IConfiguration configuration, IPasswordHasher passwordHasher)
        {
            _users = users;
            _configuration = configuration;
            _passwordHasher = passwordHasher;
        }

        public async Task<string?> LoginAsync(string login, string password, CancellationToken cancellationToken)
        {
            var user = await _users.GetByLoginAsync(login, cancellationToken);
            if (user is null) return null;
            if (!_passwordHasher.Verify(password, user.PasswordHash)) return null;

            var key = _configuration["Jwt:Key"] ?? "CHANGE_ME_DEV_KEY";
            var issuer = _configuration["Jwt:Issuer"] ?? "ArmNavigation";
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new(ClaimTypes.Name, user.Login),
                new(ClaimTypes.Role, user.Role.ToString()),
                new("org", user.MedInstitutionId.ToString())
            };

            var jwt = new JwtSecurityToken(
                issuer: issuer,
                audience: null,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(12),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        // password hashing moved to IPasswordHasher
    }
}


