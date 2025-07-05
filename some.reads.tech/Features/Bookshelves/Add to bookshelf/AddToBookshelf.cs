using System.Security.Claims;
using System.Text.Json;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using some.reads.tech.Common;
using some.reads.tech.Features.Books;
using some.reads.tech.Helpers;
using some.reads.tech.Shared;
using some.reads.tech.Shared.Dto;
using StackExchange.Redis;

namespace some.reads.tech.Features.Bookshelves.Add_to_bookshelf
{
    public static class AddToBookshelf
    {
        public static void AddAddToBookshelfEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("bookshelves", Handler).RequireAuthorization();
        }
        private static async Task<IResult> Handler(
            [FromBody] BookDto book,
            [FromQuery] string status,
            [FromQuery] string query,
            NpgsqlConnectionFactory connectionFactory,
            ClaimsPrincipal claims,
            IConnectionMultiplexer connectionMultiplexer,
            IOptions<JsonOptions> jsonOptions
            )
        {
            var redisDb = connectionMultiplexer.GetDatabase();
            
            if (!BookExists(redisDb, jsonOptions, query, book.Key))
                return Results.BadRequest(new { message = "Book seems to be invalid. Is this a mistake?" });
            
            var userIdString = claims.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value
                ?? throw new InvalidOperationException("User Id claim not present");
            
            if (!Guid.TryParse(userIdString, out var userId))
                return Results.BadRequest(new { message = "Invalid user ID format" });
            
            await using var connection = connectionFactory.Create();

            const string sql =
                @"INSERT INTO bookshelves (book_id, user_id, status, title, author_names, cover_pics) VALUES (@BookId, @UserId, @Status, @Title, @AuthorNames, @CoverPics);";

            try
            {
                var rowsAffected = await connection.ExecuteAsync(sql, new { BookId = book.Key, UserId = userId, Status = status, book.Title, book.AuthorName, book.CoverPics });
                return rowsAffected > 0 ? Results.Ok(new { message = "Book added to bookshelf" }) : Results.BadRequest(new { message = "Failed to add book to bookshelf" });
            }
            catch (Exception ex)
            {
                return PostgresExceptionHandler.Handle(ex);
            }
        }
        // This validates if the book that you are trying to add to your bookshelf exists in the Open Library API
        // However, calling this API is expensive, so we use the results from cache to make the validation faster
        // Ideally, users press "Add to library" button right after searching for the book, so the cache *will* still be valid
        // TODO: Add a fallback to actually call the Open Library API if the cache is invalid (slower case, but wont block the bookshelf add if cache cant be hit or is invalid)
        private static bool BookExists(IDatabase redisDb, IOptions<JsonOptions> jsonOptions, string query, string? key)
        {
            var searchCacheKey = $"GET:/books/search?name={query}";
            
            if (redisDb.KeyExists(searchCacheKey))
            {
                var value = redisDb.StringGet(searchCacheKey);

                var searchResponse = JsonSerializer.Deserialize<OpenLibraryResponse<BookSearchResponse>>(value, jsonOptions.Value.JsonSerializerOptions);
                
                return searchResponse.Docs.Any(x => x.Key.Equals(key));
            }

            return false;
        }
    }
}
