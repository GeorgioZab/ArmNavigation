using ArmNavigation.Infrastructure.Migrator.Extensions;
using Microsoft.Extensions.DependencyInjection;
using ArnNavigation.Application.Services;
using ArnNavigation.Application.Repositories;

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

        service.ConfigureRepositories();
        service.ConfigureServices();

        service.AddMigration(connectionStrings);
        return service;
    }

    private static void ConfigureRepositories(this IServiceCollection service)
    {
        service.AddScoped<IMedInstitutionRepository, Repositories.MedInstitutionRepository>();
        service.AddScoped<IUserRepository, Repositories.UserRepository>();
    }

    private static void ConfigureServices(this IServiceCollection service)
    {
        service.AddScoped<IMedInstitutionService, MedInstitutionService>();
        service.AddScoped<IUsersService, UsersService>();
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
}