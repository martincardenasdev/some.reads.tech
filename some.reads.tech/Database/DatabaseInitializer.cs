using Dapper;
using Microsoft.AspNetCore.Identity;
using some.reads.tech.Features.Users;
using some.reads.tech.Helpers;

namespace some.reads.tech.Database
{
    public sealed class DatabaseInitializer(NpgsqlConnectionFactory connectionFactory)
    {
        private static readonly User User = new();
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

            var hashedPassword = new PasswordHasher<User>().HashPassword(User, "password");

            var userId = await connection.ExecuteScalarAsync<Guid>(
                """
                INSERT INTO users (username, password_hash)
                VALUES ('admin', @PasswordHash)
                RETURNING id;
                """,
                new { PasswordHash = hashedPassword });

            // book_id here refers to the open library key
            await connection.ExecuteAsync(
                """
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

            await connection.ExecuteAsync(
                """
                INSERT INTO bookshelves (book_id, user_id, status, title, author_names, cover_pics)
                VALUES (@BookId, @UserId, @Status, @Title, @AuthorNames, @CoverPics)
                """,
                new
                {
                    BookId = "OL22856696M",
                    UserId = userId,
                    Status = "reading",
                    Title = "Harry Potter and the Philosopher's Stone",
                    AuthorNames = (string[]) ["J. K. Rowling"],
                    CoverPics = (string[]) ["https://covers.openlibrary.org/b/olid/OL22856696M-L.jpg"]
                });
        }
    }
}
