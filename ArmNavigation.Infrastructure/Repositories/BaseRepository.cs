using System.Data;
using Npgsql;

namespace ArmNavigation.Infrastructure.Repositories
{
    public abstract class BaseRepository
    {
        protected const string ConnectionEnv = "POSTGRES_CONNECTION_STRING";

        protected static string GetConnectionString()
        {
            var cs = Environment.GetEnvironmentVariable(ConnectionEnv);
            if (string.IsNullOrWhiteSpace(cs))
            {
                throw new InvalidOperationException($"Environment variable {ConnectionEnv} is not set.");
            }
            return cs;
        }

        protected static IDbConnection CreateConnection()
        {
            return new NpgsqlConnection(GetConnectionString());
        }
    }
}

