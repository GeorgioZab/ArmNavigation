using System.Security.Cryptography;
using System.Text;
using ArnNavigation.Application.Services;

namespace ArmNavigation.Services
{
    public sealed class PasswordHasher : IPasswordHasher
    {
        public string Hash(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(16);
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(32);
            return Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);
        }

        public bool Verify(string password, string storedHash)
        {
            var parts = storedHash.Split(':');
            if (parts.Length != 2) return false;
            var salt = Convert.FromBase64String(parts[0]);
            var expected = parts[1];
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
            var hash = Convert.ToBase64String(pbkdf2.GetBytes(32));
            return CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(expected), Encoding.UTF8.GetBytes(hash));
        }
    }
}



