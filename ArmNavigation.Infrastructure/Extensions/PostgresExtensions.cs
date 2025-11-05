using ArmNavigation.Infrastructure.Migrator.Extensions;
using Microsoft.Extensions.DependencyInjection;
using ArnNavigation.Application.Services;
using ArnNavigation.Application.Repositories;
using Npgsql;

namespace ArmNavigation.Infrastructure.Postgres.Extensions;

public static class PostgresExtensions
{
    private const string DbConnectionString = "POSTGRES_CONNECTION_STRING";

    public static IServiceCollection ConfigurePostgresInfrastructure(this IServiceCollection service)
    {
        string? connectionString = Environment.GetEnvironmentVariable(DbConnectionString);
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"Отсутствует переменная окружения {DbConnectionString}. Заполните ее и перезапустите приложение");
        }
        string connectionStrings = GetConnectionString();
        EnsureDatabaseExists(connectionStrings);

        service.ConfigureRepositories();
        service.ConfigureServices();

        service.AddMigration(connectionStrings);
        return service;
    }

    private static void ConfigureRepositories(this IServiceCollection service)
    {
        service.AddScoped<IMedInstitutionRepository, Repositories.MedInstitutionRepository>();
        service.AddScoped<IUserRepository, Repositories.UserRepository>();
        service.AddScoped<ICarRepository, Repositories.CarRepository>();
    }

    private static void ConfigureServices(this IServiceCollection service)
    {
        service.AddScoped<IMedInstitutionService, MedInstitutionService>();
        service.AddScoped<IUsersService, UsersService>();
        service.AddScoped<ICarsService, CarsService>();
    }

    private static string GetConnectionString()
    {
        string? connectionString = Environment.GetEnvironmentVariable(DbConnectionString);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"Отсутствует переменная окружения {DbConnectionString}. Заполните ее и перезапустите приложение");
        }

        return connectionString;
    }

    private static void EnsureDatabaseExists(string connectionString)
    {
        var target = new NpgsqlConnectionStringBuilder(connectionString);
        var databaseName = target.Database;
        if (string.IsNullOrWhiteSpace(databaseName))
        {
            return;
        }

        var adminDb = Environment.GetEnvironmentVariable("DATABASE");
        var adminBuilder = new NpgsqlConnectionStringBuilder(connectionString)
        {
            Database = string.IsNullOrWhiteSpace(adminDb) ? "postgres" : adminDb
        };

        using var admin = new NpgsqlConnection(adminBuilder.ConnectionString);
        admin.Open();
        using (var cmd = admin.CreateCommand())
        {
            cmd.CommandText = """
                SELECT 1 FROM pg_database WHERE datname = @name
                """;
            cmd.Parameters.AddWithValue("name", databaseName);
            var exists = cmd.ExecuteScalar() != null;
            if (!exists)
            {
                using var create = admin.CreateCommand();
                create.CommandText = $"""
                    CREATE DATABASE "{databaseName}"
                    """;
                create.ExecuteNonQuery();
            }
        }
    }
}