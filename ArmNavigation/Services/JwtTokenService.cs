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
        private const string DefaultKey = "CHANGE_ME_DEV_KEY";
        private const string DefaultIssuer = "ArmNavigation";
        private const int TokenLifetimeHours = 12;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(Guid userId, string login, int role, Guid orgId)
        {
            var tokenConfiguration = GetTokenConfiguration();
            var signingCredentials = CreateSigningCredentials(tokenConfiguration.Key);
            var claims = BuildUserClaims(userId, login, role, orgId);

            var token = CreateJwtSecurityToken(tokenConfiguration, signingCredentials, claims);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private TokenConfiguration GetTokenConfiguration()
        {
            return new TokenConfiguration(
                Key: _configuration["Jwt:Key"] ?? DefaultKey,
                Issuer: _configuration["Jwt:Issuer"] ?? DefaultIssuer
            );
        }

        private static SigningCredentials CreateSigningCredentials(string key)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            return new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        }

        private static List<Claim> BuildUserClaims(Guid userId, string login, int role, Guid orgId)
        {
            return new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userId.ToString()),
                new(ClaimTypes.Name, login),
                new(ClaimTypes.Role, role.ToString()),
                new("org", orgId.ToString())
            };
        }

        private static JwtSecurityToken CreateJwtSecurityToken(
            TokenConfiguration config,
            SigningCredentials credentials,
            List<Claim> claims)
        {
            return new JwtSecurityToken(
                issuer: config.Issuer,
                audience: null,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(TokenLifetimeHours),
                signingCredentials: credentials);
        }

        private record TokenConfiguration(string Key, string Issuer);
    }
}
