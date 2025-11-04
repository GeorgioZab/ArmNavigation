using System.Data;
using ArnNavigation.Application.Repositories;
using ArmNavigation.Domain.Enums;
using ArmNavigation.Domain.Models;
using Dapper;
using Npgsql;

namespace ArmNavigation.Infrastructure.Repositories
{
    public sealed class UserRepository : IUserRepository
    {
        private const string ConnectionEnv = "POSTGRES_CONNECTION_STRING";

        private static string GetConnectionString()
        {
            var cs = Environment.GetEnvironmentVariable(ConnectionEnv);
            if (string.IsNullOrWhiteSpace(cs))
            {
                throw new InvalidOperationException($"Environment variable {ConnectionEnv} is not set.");
            }
            return cs;
        }

        private static IDbConnection CreateConnection() => new NpgsqlConnection(GetConnectionString());

        private sealed class DbUserRow
        {
            public Guid UserId { get; init; }
            public string Login { get; init; }
            public string PasswordHash { get; init; }
            public Guid MedInstitutionId { get; init; }
            public bool IsRemoved { get; init; }
            public string RoleName { get; init; }
        }

        public async Task<User?> GetByLoginAsync(string login, CancellationToken cancellationToken)
        {
            const string sql =  "SELECT u.\"" +
                "UserId\", u.\"Login\", u.\"Password\" AS \"PasswordHash\", u.\"MedInstitutionId\", " +
                "u.\"IsRemoved\", COALESCE(ur.\"Name\", 'Dispatcher') AS \"RoleName\" " +
                "FROM \"Users\" u LEFT JOIN \"UserRoles\" ur ON ur.\"UserRoleId\" = u.\"UserRoleId\" " +
                "WHERE u.\"Login\" = @login AND u.\"IsRemoved\" = false";

            await using var conn = (NpgsqlConnection)CreateConnection();
            var row = await conn.QuerySingleOrDefaultAsync<DbUserRow>(new CommandDefinition(sql, new { login }, cancellationToken: cancellationToken));
            if (row == null) return null;
            return new User
            {
                UserId = row.UserId,
                Login = row.Login,
                PasswordHash = row.PasswordHash,
                MedInstitutionId = row.MedInstitutionId,
                Role = Enum.TryParse<Role>(row.RoleName, out var r) ? r : Role.Dispatcher,
                IsRemoved = row.IsRemoved
            };
        }

        public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            const string sql = "SELECT u.\"UserId\", u.\"Login\", u.\"Password\" AS \"PasswordHash\", u.\"MedInstitutionId\", u.\"IsRemoved\",COALESCE(ur.\"Name\", 'Dispatcher') AS \"RoleName\"" +
                "FROM \"Users\" u " +
                "LEFT JOIN \"UserRoles\" ur ON ur.\"UserRoleId\" = u.\"UserRoleId\" " +
                "WHERE u.\"UserId\" = @id AND u.\"IsRemoved\" = false";

            await using var conn = (NpgsqlConnection)CreateConnection();
            var row = await conn.QuerySingleOrDefaultAsync<DbUserRow>(new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken));
            if (row == null) return null;
            return new User
            {
                UserId = row.UserId,
                Login = row.Login,
                PasswordHash = row.PasswordHash,
                MedInstitutionId = row.MedInstitutionId,
                Role = Enum.TryParse<Role>(row.RoleName, out var r) ? r : Role.Dispatcher,
                IsRemoved = row.IsRemoved
            };
        }

        public async Task<Guid> CreateAsync(User user, CancellationToken cancellationToken)
        {
            var id = user.UserId != Guid.Empty ? user.UserId : Guid.NewGuid();
            const string sql = "INSERT INTO \"Users\" (\"UserId\", \"Login\", \"Password\", \"IsRemoved\", \"UserRoleId\", \"MedInstitutionId\") " +
                "VALUES (@id, @login, @password, false, " +
                "(SELECT ur.\"UserRoleId\" FROM \"UserRoles\" ur WHERE ur.\"Name\" = @roleName), " +
                "@orgId)";
            await using var conn = (NpgsqlConnection)CreateConnection();
            await conn.ExecuteAsync(new CommandDefinition(sql, new { id, login = user.Login, password = user.PasswordHash, roleName = user.Role.ToString(), orgId = user.MedInstitutionId }, cancellationToken: cancellationToken));
            return id;
        }

        public async Task<bool> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            const string sql = "UPDATE \"Users\" SET \"Login\" = @login, \"Password\" = @password, " +
                "\"UserRoleId\" = (SELECT ur.\"UserRoleId\" FROM \"UserRoles\" ur WHERE ur.\"Name\" = @roleName), " +
                "\"MedInstitutionId\" = @orgId " +
                "WHERE \"UserId\" = @id AND \"IsRemoved\" = false";
            await using var conn = (NpgsqlConnection)CreateConnection();
            var affected = await conn.ExecuteAsync(new CommandDefinition(sql, new { id = user.UserId, login = user.Login, password = user.PasswordHash, roleName = user.Role.ToString(), orgId = user.MedInstitutionId }, cancellationToken: cancellationToken));
            return affected > 0;
        }

        public async Task<bool> SoftDeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            const string sql = "UPDATE \"Users\" SET \"IsRemoved\" = true WHERE \"UserId\" = @id AND \"IsRemoved\" = false";
            await using var conn = (NpgsqlConnection)CreateConnection();
            var affected = await conn.ExecuteAsync(new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken));
            return affected > 0;
        }

        public async Task<IEnumerable<User>> GetAllByOrgAsync(Guid? medInstitutionId, CancellationToken cancellationToken)
        {
            var where = medInstitutionId.HasValue ? "AND u.\"MedInstitutionId\" = @orgId" : string.Empty;
            var sql = $"SELECT u.\"UserId\", u.\"Login\", u.\"Password\" AS \"PasswordHash\", u.\"MedInstitutionId\", u.\"IsRemoved\", " +
                $"COALESCE(ur.\"Name\", 'Dispatcher') AS \"RoleName\" " +
                $"FROM \"Users\" u " +
                $"LEFT JOIN \"UserRoles\" ur ON ur.\"UserRoleId\" = u.\"UserRoleId\" " +
                $"WHERE u.\"IsRemoved\" = false {where}";
            await using var conn = (NpgsqlConnection)CreateConnection();
            var rows = await conn.QueryAsync<DbUserRow>(new CommandDefinition(sql, new { orgId = medInstitutionId }, cancellationToken: cancellationToken));
            return rows.Select(row => new User
            {
                UserId = row.UserId,
                Login = row.Login,
                PasswordHash = row.PasswordHash,
                MedInstitutionId = row.MedInstitutionId,
                Role = Enum.TryParse<Role>(row.RoleName, out var r) ? r : Role.Dispatcher,
                IsRemoved = row.IsRemoved
            });
        }
    }
}


