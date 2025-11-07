using ArmNavigation.Domain.Models;
using ArnNavigation.Application.Repositories;
using Dapper;
using Npgsql;
using System.Data;
using System.Threading;

namespace ArmNavigation.Infrastructure.Postgres.Repositories
{
    public sealed class CarRepository : BaseRepository, ICarRepository
    {
        public async Task<IEnumerable<Car>> GetAllByOrgAsync(Guid? medInstitutionId, CancellationToken token)
        {
            const string sql = """
            SELECT c."CarId", c."RegNum", c."MedInstitutionId", c."Gps-tracker" AS "GpsTracker", c."IsRemoved"
            FROM "Cars" c
            WHERE c."IsRemoved" = false
            """;

            var parameters = new DynamicParameters();
            if (medInstitutionId.HasValue)
                parameters.Add("orgId", medInstitutionId.Value);

            return await ExecuteQueryAsync<Car>(sql, parameters, token);
        }

        public async Task<Car?> GetByIdAsync(Guid id, CancellationToken token)
        {
            const string sql = """
            SELECT c."CarId", c."RegNum", c."MedInstitutionId", c."Gps-tracker" AS "GpsTracker", c."IsRemoved"
            FROM "Cars" c
            WHERE c."CarId" = @id AND c."IsRemoved" = false
            """;
            return await ExecuteQuerySingleOrDefaultAsync<Car>(sql, new { id }, token);
        }

        public async Task<Guid> CreateAsync(Car car, CancellationToken token)
        {
            var id = car.CarId != Guid.Empty ? car.CarId : Guid.NewGuid();
            const string sql = """
            INSERT INTO "Cars" ("CarId", "RegNum", "Gps-tracker", "IsRemoved", "MedInstitutionId")
            VALUES (@id, @reg, @gps, false, @orgId)
            RETURNING "CarId"
            """;

            return await ExecuteScalarAsync<Guid>(sql, new
            {
                id,
                reg = car.RegNum,
                gps = car.GpsTracker,
                orgId = car.MedInstitutionId
            }, token);
        }

        public async Task<Car> UpdateAsync(Car car, CancellationToken token)
        {
            const string sql = """
            UPDATE "Cars"
            SET "RegNum" = @reg, "Gps-tracker" = @gps, "MedInstitutionId" = @orgId
            WHERE "CarId" = @id AND "IsRemoved" = false
            """;

            await ExecuteNonQueryAsync<Car>(sql, new
            {
                id = car.CarId,
                reg = car.RegNum,
                gps = car.GpsTracker,
                orgId = car.MedInstitutionId
            }, token);

            return car;
        }

        public async Task<Car> SoftDeleteAsync(Guid id, CancellationToken token)
        {
            const string sql = """
            UPDATE "Cars"
            SET "IsRemoved" = true
            WHERE "CarId" = @id AND "IsRemoved" = false
            """;

            return await ExecuteNonQueryAsync<Car>(sql, new { id }, token);
        }

        public async Task<IEnumerable<Car>> GetAsync(string query, Guid? medInstitutionId, CancellationToken token)
        {
            const string sql = """
                SELECT c."CarId", c."RegNum", c."MedInstitutionId", c."Gps-tracker" AS "GpsTracker", c."IsRemoved"
                FROM "Cars" c
                WHERE c."IsRemoved" = false
                  AND (c."RegNum" ILIKE @q OR c."Gps-tracker" ILIKE @q)
                """;

            var parameters = new DynamicParameters();
            if (medInstitutionId.HasValue)
                parameters.Add("orgId", medInstitutionId.Value);

            return await ExecuteQueryAsync<Car>(sql, parameters, token);
        }

        public async Task<Car> BindTrackerAsync(Guid carId, string tracker, CancellationToken token)
        {
            const string sql = """
            UPDATE "Cars"
            SET "Gps-tracker" = @gps
            WHERE "CarId" = @id AND "IsRemoved" = false
            """;

            return await ExecuteNonQueryAsync<Car>(sql, new { id = carId, gps = tracker }, token);
        }

        public async Task<Car> UnbindTrackerAsync(Guid carId, CancellationToken token)
        {
            const string sql = """
            UPDATE "Cars"
            SET "Gps-tracker" = NULL
            WHERE "CarId" = @id AND "IsRemoved" = false
            """;

            return await ExecuteNonQueryAsync<Car>(sql, new { id = carId }, token);
        }
    }
}