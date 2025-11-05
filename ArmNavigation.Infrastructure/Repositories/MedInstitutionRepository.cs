using System.Data;
using ArnNavigation.Application.Repositories;
using ArmNavigation.Domain.Models;
using Dapper;
using Npgsql;

namespace ArmNavigation.Infrastructure.Repositories
{
    public sealed class MedInstitutionRepository : BaseRepository, IMedInstitutionRepository
    {

        public async Task<IEnumerable<MedInstitution>> GetAllAsync(CancellationToken cancellationToken)
        {
            const string sql = """
                SELECT "MedInstitutionId", "Name", "IsRemoved"
                FROM "MedInstitutions"
                WHERE "IsRemoved" = false
                """;
            await using var conn = (NpgsqlConnection)CreateConnection();
            return await conn.QueryAsync<MedInstitution>(new CommandDefinition(sql, cancellationToken: cancellationToken));
        }

        public async Task<IEnumerable<MedInstitution>> GetAllByNameAsync(string? nameFilter, CancellationToken cancellationToken)
        {
            await using var conn = (NpgsqlConnection)CreateConnection();
            if (string.IsNullOrWhiteSpace(nameFilter))
            {
                return await GetAllAsync(cancellationToken);
            }

            const string sql = """
                SELECT "MedInstitutionId", "Name", "IsRemoved"
                FROM "MedInstitutions"
                WHERE "IsRemoved" = false AND "Name" ILIKE @name
                """;
            return await conn.QueryAsync<MedInstitution>(new CommandDefinition(sql, new { name = $"%{nameFilter}%" }, cancellationToken: cancellationToken));
        }

        public async Task<MedInstitution?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            const string sql = """
                SELECT "MedInstitutionId", "Name", "IsRemoved"
                FROM "MedInstitutions"
                WHERE "MedInstitutionId" = @id AND "IsRemoved" = false
                """;
            await using var conn = (NpgsqlConnection)CreateConnection();
            return await conn.QuerySingleOrDefaultAsync<MedInstitution>(new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken));
        }

        public async Task<Guid> CreateAsync(MedInstitution medInstitution, CancellationToken cancellationToken)
        {
            var id = medInstitution.MedInstitutionId != Guid.Empty ? medInstitution.MedInstitutionId : Guid.NewGuid();
            const string sql = """
                INSERT INTO "MedInstitutions" ("MedInstitutionId", "Name", "IsRemoved")
                VALUES (@id, @name, false)
                """;
            await using var conn = (NpgsqlConnection)CreateConnection();
            await conn.ExecuteAsync(new CommandDefinition(sql, new { id, name = medInstitution.Name }, cancellationToken: cancellationToken));
            return id;
        }

        public async Task<bool> UpdateAsync(MedInstitution medInstitution, CancellationToken cancellationToken)
        {
            const string sql = """
                UPDATE "MedInstitutions"
                SET "Name" = @name
                WHERE "MedInstitutionId" = @id AND "IsRemoved" = false
                """;
            await using var conn = (NpgsqlConnection)CreateConnection();
            var affected = await conn.ExecuteAsync(new CommandDefinition(sql, new { id = medInstitution.MedInstitutionId, name = medInstitution.Name }, cancellationToken: cancellationToken));
            return affected > 0;
        }

        public async Task<bool> SoftDeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            const string sql = """
                UPDATE "MedInstitutions"
                SET "IsRemoved" = true
                WHERE "MedInstitutionId" = @id AND "IsRemoved" = false
                """;
            await using var conn = (NpgsqlConnection)CreateConnection();
            var affected = await conn.ExecuteAsync(new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken));
            return affected > 0;
        }
    }
}



