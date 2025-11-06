using ArnNavigation.Application.Repositories;
using ArmNavigation.Domain.Models;
using Dapper;

namespace ArmNavigation.Infrastructure.Repositories;
        private const string ConnectionEnv = "POSTGRES_CONNECTION_STRING";
public sealed class MedInstitutionRepository : BaseRepository, IMedInstitutionRepository
{
    public async Task<IEnumerable<MedInstitution>> GetAllAsync(CancellationToken ct)
    {
        const string sql = """
            SELECT "MedInstitutionId", "Name", "IsRemoved"
            FROM "MedInstitutions"
            WHERE "IsRemoved" = false
            """;

        return await QueryAsync<MedInstitution>(sql, ct: ct);
    }

    public async Task<IEnumerable<MedInstitution>> GetAllByNameAsync(string? nameFilter, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(nameFilter))
            return await GetAllAsync(ct);

        const string sql = """
            SELECT "MedInstitutionId", "Name", "IsRemoved"
            FROM "MedInstitutions"
            WHERE "IsRemoved" = false AND "Name" ILIKE @name
            """;

        var parameters = new DynamicParameters();
        parameters.Add("name", $"%{nameFilter}%");

        return await QueryAsync<MedInstitution>(sql, parameters, ct);
    }
        }

    public async Task<MedInstitution?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        const string sql = """
            SELECT "MedInstitutionId", "Name", "IsRemoved"
            FROM "MedInstitutions"
            WHERE "MedInstitutionId" = @id AND "IsRemoved" = false
            """;
        var parameters = new DynamicParameters();
        parameters.Add("id", id);
        }
        return await QuerySingleOrDefaultAsync<MedInstitution>(sql, parameters, ct);
    }
        }
    public async Task<Guid> CreateAsync(MedInstitution medInstitution, CancellationToken ct)
    {
        var id = medInstitution.MedInstitutionId != Guid.Empty ? medInstitution.MedInstitutionId : Guid.NewGuid();
        const string sql = """
            INSERT INTO "MedInstitutions" ("MedInstitutionId", "Name", "IsRemoved")
            VALUES (@id, @name, false)
            """;

        var parameters = new DynamicParameters();
        parameters.Add("id", id);
        parameters.Add("name", medInstitution.Name);

        await ExecuteAsync(sql, parameters, ct);
        return id;
    }
        }
    public async Task<bool> UpdateAsync(MedInstitution medInstitution, CancellationToken ct)
    {
        const string sql = """
            UPDATE "MedInstitutions"
            SET "Name" = @name
            WHERE "MedInstitutionId" = @id AND "IsRemoved" = false
            """;

        var parameters = new DynamicParameters();
        parameters.Add("id", medInstitution.MedInstitutionId);
        parameters.Add("name", medInstitution.Name);

        var affected = await ExecuteAsync(sql, parameters, ct);
        return affected > 0;
    }
        }
    public async Task<bool> SoftDeleteAsync(Guid id, CancellationToken ct)
    {
        const string sql = """
            UPDATE "MedInstitutions"
            SET "IsRemoved" = true
            WHERE "MedInstitutionId" = @id AND "IsRemoved" = false
            """;

        var parameters = new DynamicParameters();
        parameters.Add("id", id);

        var affected = await ExecuteAsync(sql, parameters, ct);
        return affected > 0;
    }
}
