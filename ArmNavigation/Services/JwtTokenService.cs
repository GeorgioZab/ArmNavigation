using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ArmNavigation.Services
{
    public sealed class JwtTokenService
    {
        private readonly IConfiguration _configuration;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(Guid userId, string login, int role, Guid orgId)
        {
            var key = _configuration["Jwt:Key"] ?? "CHANGE_ME_DEV_KEY";
            var issuer = _configuration["Jwt:Issuer"] ?? "ArmNavigation";
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userId.ToString()),
                new(ClaimTypes.Name, login),
                new(ClaimTypes.Role, role.ToString()),
                new("org", orgId.ToString())
            };

            var jwt = new JwtSecurityToken(
                issuer: issuer,
                audience: null,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(12),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}
