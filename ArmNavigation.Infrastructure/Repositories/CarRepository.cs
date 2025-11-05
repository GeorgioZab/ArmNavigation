using System.Data;
using ArnNavigation.Application.Repositories;
using ArmNavigation.Domain.Models;
using Dapper;
using Npgsql;

namespace ArmNavigation.Infrastructure.Repositories
{
    public sealed class CarRepository : BaseRepository, ICarRepository
    {

        public async Task<IEnumerable<Car>> GetAllByOrgAsync(Guid? medInstitutionId, CancellationToken ct)
        {
            var where = medInstitutionId.HasValue ? "AND c.\"MedInstitutionId\" = @orgId" : string.Empty;
            var sql = $"""
                SELECT c."CarId", c."RegNum", c."MedInstitutionId", c."Gps-tracker" AS "GpsTracker", c."IsRemoved"
                FROM "Cars" c
                WHERE c."IsRemoved" = false {where}
                """;
            await using var conn = (NpgsqlConnection)CreateConnection();
            return await conn.QueryAsync<Car>(new CommandDefinition(sql, new { orgId = medInstitutionId }, cancellationToken: ct));
        }

        public async Task<Car?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            const string sql = """
                SELECT c."CarId", c."RegNum", c."MedInstitutionId", c."Gps-tracker" AS "GpsTracker", c."IsRemoved"
                FROM "Cars" c
                WHERE c."CarId" = @id AND c."IsRemoved" = false
                """;
            await using var conn = (NpgsqlConnection)CreateConnection();
            return await conn.QuerySingleOrDefaultAsync<Car>(new CommandDefinition(sql, new { id }, cancellationToken: ct));
        }

        public async Task<Guid> CreateAsync(Car car, CancellationToken ct)
        {
            var id = car.CarId != Guid.Empty ? car.CarId : Guid.NewGuid();
            const string sql = """
                INSERT INTO "Cars" ("CarId", "RegNum", "Gps-tracker", "IsRemoved", "MedInstitutionId")
                VALUES (@id, @reg, @gps, false, @orgId)
                """;
            await using var conn = (NpgsqlConnection)CreateConnection();
            await conn.ExecuteAsync(new CommandDefinition(sql, new { id, reg = car.RegNum, gps = car.GpsTracker, orgId = car.MedInstitutionId }, cancellationToken: ct));
            return id;
        }

        public async Task<bool> UpdateAsync(Car car, CancellationToken ct)
        {
            const string sql = """
                UPDATE "Cars"
                SET "RegNum" = @reg, "Gps-tracker" = @gps, "MedInstitutionId" = @orgId
                WHERE "CarId" = @id AND "IsRemoved" = false
                """;
            await using var conn = (NpgsqlConnection)CreateConnection();
            var affected = await conn.ExecuteAsync(new CommandDefinition(sql, new { id = car.CarId, reg = car.RegNum, gps = car.GpsTracker, orgId = car.MedInstitutionId }, cancellationToken: ct));
            return affected > 0;
        }

        public async Task<bool> SoftDeleteAsync(Guid id, CancellationToken ct)
        {
            const string sql = """
                UPDATE "Cars"
                SET "IsRemoved" = true
                WHERE "CarId" = @id AND "IsRemoved" = false
                """;
            await using var conn = (NpgsqlConnection)CreateConnection();
            var affected = await conn.ExecuteAsync(new CommandDefinition(sql, new { id }, cancellationToken: ct));
            return affected > 0;
        }

        public async Task<IEnumerable<Car>> SearchAsync(string query, Guid? medInstitutionId, CancellationToken ct)
        {
            var whereOrg = medInstitutionId.HasValue ? "AND c.\"MedInstitutionId\" = @orgId" : string.Empty;
            var sql = $"""
                SELECT c."CarId", c."RegNum", c."MedInstitutionId", c."Gps-tracker" AS "GpsTracker", c."IsRemoved"
                FROM "Cars" c
                WHERE c."IsRemoved" = false
                AND (c."RegNum" ILIKE @q OR c."Gps-tracker" ILIKE @q) {whereOrg}
                """;
            await using var conn = (NpgsqlConnection)CreateConnection();
            return await conn.QueryAsync<Car>(new CommandDefinition(sql, new { q = $"%{query}%", orgId = medInstitutionId }, cancellationToken: ct));
        }

        public async Task<bool> BindTrackerAsync(Guid carId, string tracker, CancellationToken ct)
        {
            const string sql = """
                UPDATE "Cars"
                SET "Gps-tracker" = @gps
                WHERE "CarId" = @id AND "IsRemoved" = false
                """;
            await using var conn = (NpgsqlConnection)CreateConnection();
            var affected = await conn.ExecuteAsync(new CommandDefinition(sql, new { id = carId, gps = tracker }, cancellationToken: ct));
            return affected > 0;
        }

        public async Task<bool> UnbindTrackerAsync(Guid carId, CancellationToken ct)
        {
            const string sql = """
                UPDATE "Cars"
                SET "Gps-tracker" = NULL
                WHERE "CarId" = @id AND "IsRemoved" = false
                """;
            await using var conn = (NpgsqlConnection)CreateConnection();
            var affected = await conn.ExecuteAsync(new CommandDefinition(sql, new { id = carId }, cancellationToken: ct));
            return affected > 0;
        }
    }
}



