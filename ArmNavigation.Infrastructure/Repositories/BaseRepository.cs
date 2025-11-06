using System.Data;
using Dapper;
using Npgsql;

namespace ArmNavigation.Infrastructure.Repositories;

    public abstract class BaseRepository
    {
    private const string ConnectionEnv = "POSTGRES_CONNECTION_STRING";
    private const int DefaultTimeoutSeconds = 30;

    private static string GetConnectionString()
        {
            var cs = Environment.GetEnvironmentVariable(ConnectionEnv);
        return string.IsNullOrWhiteSpace(cs)
            ? throw new InvalidOperationException($"Environment variable '{ConnectionEnv}' is not set.")
            : cs;
    }

    private static NpgsqlConnection CreateConnection() => new(GetConnectionString());

    protected static async Task<T[]> QueryAsync<T>(
        string sql,
        DynamicParameters? parameters = null,
        CancellationToken ct = default,
        int? timeout = null)
            {
        var command = new CommandDefinition(sql, parameters, cancellationToken: ct, commandTimeout: timeout ?? DefaultTimeoutSeconds);
        await using var conn = CreateConnection();
        var result = await conn.QueryAsync<T>(command);
        return result.ToArray();
            }

    protected static async Task<T?> QuerySingleOrDefaultAsync<T>(
        string sql,
        DynamicParameters? parameters = null,
        CancellationToken ct = default,
        int? timeout = null)
    {
        var command = new CommandDefinition(sql, parameters, cancellationToken: ct, commandTimeout: timeout ?? DefaultTimeoutSeconds);
        await using var conn = CreateConnection();
        return await conn.QuerySingleOrDefaultAsync<T>(command);
        }

    protected static async Task<int> ExecuteAsync(
        string sql,
        DynamicParameters? parameters = null,
        CancellationToken ct = default,
        int? timeout = null)
        {
        var command = new CommandDefinition(sql, parameters, cancellationToken: ct, commandTimeout: timeout ?? DefaultTimeoutSeconds);
        await using var conn = CreateConnection();
        return await conn.ExecuteAsync(command);
        }

    protected static async Task<T> ExecuteScalarAsync<T>(
        string sql,
        DynamicParameters? parameters = null,
        CancellationToken ct = default,
        int? timeout = null)
    {
        var command = new CommandDefinition(sql, parameters, cancellationToken: ct, commandTimeout: timeout ?? DefaultTimeoutSeconds);
        await using var conn = CreateConnection();
        return await conn.ExecuteScalarAsync<T>(command);
    }
}

