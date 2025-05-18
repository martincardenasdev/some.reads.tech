using Dapper;
using some.reads.tech.Helpers;

namespace some.reads.tech.Database
{
    public sealed class DatabaseInitializer(NpgsqlConnectionFactory connectionFactory)
    {
        public async Task Initialize()
        {
            using var connection = connectionFactory.Create();
            await connection.ExecuteAsync(
                """
                CREATE TABLE IF NOT EXISTS users (
                id UUID PRIMARY KEY DEFAULT gen_random_uuid(), 
                username VARCHAR(50) NOT NULL UNIQUE,
                password_hash VARCHAR(255) NOT NULL
                )
                """);
        }
    }
}
