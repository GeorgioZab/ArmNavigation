using System.Data;
using ArnNavigation.Application.Repositories;
using ArmNavigation.Domain.Enums;
using ArmNavigation.Domain.Models;
using Dapper;
using Npgsql;

namespace ArmNavigation.Infrastructure.Repositories
{
    public sealed class UserRepository : BaseRepository, IUserRepository
    {

        private sealed class DbUserRow
        {
            public Guid UserId { get; init; }
            public string Login { get; init; }
            public string PasswordHash { get; init; }
            public Guid MedInstitutionId { get; init; }
            public bool IsRemoved { get; init; }
            public int Role { get; init; }
        }

        public async Task<User?> GetByLoginAsync(string login, CancellationToken cancellationToken)
        {
            const string sql = """
                SELECT u."UserId", u."Login", u."Password" AS "PasswordHash", u."MedInstitutionId",
                       u."IsRemoved", u."Role"
                FROM "Users" u
                WHERE u."Login" = @login AND u."IsRemoved" = false
                """;

            await using var conn = (NpgsqlConnection)CreateConnection();
            var row = await conn.QuerySingleOrDefaultAsync<DbUserRow>(new CommandDefinition(sql, new { login }, cancellationToken: cancellationToken));
            if (row == null) return null;
            return new User
            {
                UserId = row.UserId,
                Login = row.Login,
                PasswordHash = row.PasswordHash,
                MedInstitutionId = row.MedInstitutionId,
                Role = row.Role,
                IsRemoved = row.IsRemoved
            };
        }

        public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            const string sql = """
                SELECT u."UserId", u."Login", u."Password" AS "PasswordHash", u."MedInstitutionId", u."IsRemoved", u."Role"
                FROM "Users" u
                WHERE u."UserId" = @id AND u."IsRemoved" = false
                """;

            await using var conn = (NpgsqlConnection)CreateConnection();
            var row = await conn.QuerySingleOrDefaultAsync<DbUserRow>(new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken));
            if (row == null) return null;
            return new User
            {
                UserId = row.UserId,
                Login = row.Login,
                PasswordHash = row.PasswordHash,
                MedInstitutionId = row.MedInstitutionId,
                Role = row.Role,
                IsRemoved = row.IsRemoved
            };
        }

        public async Task<Guid> CreateAsync(User user, CancellationToken cancellationToken)
        {
            var id = user.UserId != Guid.Empty ? user.UserId : Guid.NewGuid();
            const string sql = """
                INSERT INTO "Users" ("UserId", "Login", "Password", "IsRemoved", "Role", "MedInstitutionId")
                VALUES (@id, @login, @password, false, @role, @orgId)
                """;
            await using var conn = (NpgsqlConnection)CreateConnection();
            await conn.ExecuteAsync(new CommandDefinition(sql, new { id, login = user.Login, password = user.PasswordHash, role = user.Role, orgId = user.MedInstitutionId }, cancellationToken: cancellationToken));
            return id;
        }

        public async Task<bool> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            const string sql = """
                UPDATE "Users"
                SET "Login" = @login, "Password" = @password,
                    "Role" = @role,
                    "MedInstitutionId" = @orgId
                WHERE "UserId" = @id AND "IsRemoved" = false
                """;
            await using var conn = (NpgsqlConnection)CreateConnection();
            var affected = await conn.ExecuteAsync(new CommandDefinition(sql, new { id = user.UserId, login = user.Login, password = user.PasswordHash, role = user.Role, orgId = user.MedInstitutionId }, cancellationToken: cancellationToken));
            return affected > 0;
        }

        public async Task<bool> SoftDeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            const string sql = """
                UPDATE "Users"
                SET "IsRemoved" = true
                WHERE "UserId" = @id AND "IsRemoved" = false
                """;
            await using var conn = (NpgsqlConnection)CreateConnection();
            var affected = await conn.ExecuteAsync(new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken));
            return affected > 0;
        }

        public async Task<IEnumerable<User>> GetAllByOrgAsync(Guid? medInstitutionId, CancellationToken cancellationToken)
        {
            var where = medInstitutionId.HasValue ? "AND u.\"MedInstitutionId\" = @orgId" : string.Empty;
            var sql = $"""
                SELECT u."UserId", u."Login", u."Password" AS "PasswordHash", u."MedInstitutionId", u."IsRemoved", u."Role"
                FROM "Users" u
                WHERE u."IsRemoved" = false {where}
                """;
            await using var conn = (NpgsqlConnection)CreateConnection();
            var rows = await conn.QueryAsync<DbUserRow>(new CommandDefinition(sql, new { orgId = medInstitutionId }, cancellationToken: cancellationToken));
            return rows.Select(row => new User
            {
                UserId = row.UserId,
                Login = row.Login,
                PasswordHash = row.PasswordHash,
                MedInstitutionId = row.MedInstitutionId,
                Role = row.Role,
                IsRemoved = row.IsRemoved
            });
        }
    }
}


