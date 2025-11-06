using System.Data;
using ArnNavigation.Application.Repositories;
using ArmNavigation.Domain.Models;
using Dapper;
using Npgsql;

namespace ArmNavigation.Infrastructure.Repositories;

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

        var parameters = new DynamicParameters();
        if (medInstitutionId.HasValue)
            parameters.Add("orgId", medInstitutionId.Value);

        var result = await QueryAsync<Car>(sql, parameters, ct);
        return result;
        }

        public async Task<Car?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            const string sql = """
                SELECT c."CarId", c."RegNum", c."MedInstitutionId", c."Gps-tracker" AS "GpsTracker", c."IsRemoved"
                FROM "Cars" c
                WHERE c."CarId" = @id AND c."IsRemoved" = false
                """;

        var parameters = new DynamicParameters();
        parameters.Add("id", id);

        return await QuerySingleOrDefaultAsync<Car>(sql, parameters, ct);
        }

        public async Task<Guid> CreateAsync(Car car, CancellationToken ct)
        {
            var id = car.CarId != Guid.Empty ? car.CarId : Guid.NewGuid();
            const string sql = """
                INSERT INTO "Cars" ("CarId", "RegNum", "Gps-tracker", "IsRemoved", "MedInstitutionId")
                VALUES (@id, @reg, @gps, false, @orgId)
                """;

        var parameters = new DynamicParameters();
        parameters.Add("id", id);
        parameters.Add("reg", car.RegNum);
        parameters.Add("gps", car.GpsTracker);
        parameters.Add("orgId", car.MedInstitutionId);

        await ExecuteAsync(sql, parameters, ct);
            return id;
        }

        public async Task<bool> UpdateAsync(Car car, CancellationToken ct)
        {
            const string sql = """
                UPDATE "Cars"
                SET "RegNum" = @reg, "Gps-tracker" = @gps, "MedInstitutionId" = @orgId
                WHERE "CarId" = @id AND "IsRemoved" = false
                """;

        var parameters = new DynamicParameters();
        parameters.Add("id", car.CarId);
        parameters.Add("reg", car.RegNum);
        parameters.Add("gps", car.GpsTracker);
        parameters.Add("orgId", car.MedInstitutionId);

        var affected = await ExecuteAsync(sql, parameters, ct);
            return affected > 0;
        }

        public async Task<bool> SoftDeleteAsync(Guid id, CancellationToken ct)
        {
            const string sql = """
                UPDATE "Cars"
                SET "IsRemoved" = true
                WHERE "CarId" = @id AND "IsRemoved" = false
                """;

        var parameters = new DynamicParameters();
        parameters.Add("id", id);

        var affected = await ExecuteAsync(sql, parameters, ct);
            return affected > 0;
        }

    public async Task<IEnumerable<Car>> GetAsync(string query, Guid? medInstitutionId, CancellationToken ct)
        {
            var whereOrg = medInstitutionId.HasValue ? "AND c.\"MedInstitutionId\" = @orgId" : string.Empty;
            var sql = $"""
                SELECT c."CarId", c."RegNum", c."MedInstitutionId", c."Gps-tracker" AS "GpsTracker", c."IsRemoved"
                FROM "Cars" c
                WHERE c."IsRemoved" = false
                AND (c."RegNum" ILIKE @q OR c."Gps-tracker" ILIKE @q) {whereOrg}
                """;

        var parameters = new DynamicParameters();
        parameters.Add("q", $"%{query}%");
        if (medInstitutionId.HasValue)
            parameters.Add("orgId", medInstitutionId.Value);

        return await QueryAsync<Car>(sql, parameters, ct);
        }

        public async Task<bool> BindTrackerAsync(Guid carId, string tracker, CancellationToken ct)
        {
            const string sql = """
                UPDATE "Cars"
                SET "Gps-tracker" = @gps
                WHERE "CarId" = @id AND "IsRemoved" = false
                """;

        var parameters = new DynamicParameters();
        parameters.Add("id", carId);
        parameters.Add("gps", tracker);

        var affected = await ExecuteAsync(sql, parameters, ct);
            return affected > 0;
        }

        public async Task<bool> UnbindTrackerAsync(Guid carId, CancellationToken ct)
        {
            const string sql = """
                UPDATE "Cars"
                SET "Gps-tracker" = NULL
                WHERE "CarId" = @id AND "IsRemoved" = false
                """;

        var parameters = new DynamicParameters();
        parameters.Add("id", carId);

        var affected = await ExecuteAsync(sql, parameters, ct);
            return affected > 0;
        }
    }
}



