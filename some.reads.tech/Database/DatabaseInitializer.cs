using Dapper;
using some.reads.tech.Helpers;

namespace some.reads.tech.Database
{
    public sealed class DatabaseInitializer(NpgsqlConnectionFactory connectionFactory)
    {
        public async Task Initialize()
        {
            await using var connection = connectionFactory.Create();
            await connection.ExecuteAsync(
                """
                CREATE TABLE IF NOT EXISTS users (
                id UUID PRIMARY KEY DEFAULT gen_random_uuid(), 
                username VARCHAR(25) NOT NULL UNIQUE,
                password_hash VARCHAR(255) NOT NULL
                )
                """);

            // book_id here refers to the open library key 
            await connection.ExecuteAsync("""
                CREATE TABLE IF NOT EXISTS bookshelves (
                id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                book_id VARCHAR(50) NOT NULL UNIQUE,
                user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
                status TEXT NOT NULL CHECK (status IN ('to_read', 'reading', 'read', 'dropped')),
                title TEXT NOT NULL,
                author_names TEXT[] NOT NULL,
                cover_pics TEXT[] NOT NULL,
                added_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                modified_at TIMESTAMP,
                UNIQUE(user_id, book_id)
                ) 
                """);
        }
    }
}
