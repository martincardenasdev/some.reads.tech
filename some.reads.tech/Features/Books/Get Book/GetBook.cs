using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using some.reads.tech.Features.Books.Get_Book;
using some.reads.tech.Services;

namespace some.reads.tech.Features.Books
{
    public static class GetBook
    {
        public static void AddGetBookEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("books/{*key}", Handler);
        }

        private static async Task<IResult> Handler(
            [FromRoute] string key,
            [FromServices] OpenLibraryService openLibraryService,
            [FromServices] IMemoryCache memoryCache
            )
        {
            string cacheKey = $"books_{key}";

            var response = await memoryCache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

                return await openLibraryService.GetFromJsonAsync<BookResponse?>($"{key}.json");
            });

            if (response is null) return Results.NotFound(new { message = "Book not found" });

            return Results.Ok(response.Adapt<BookDto>());
        }

        public static string[] GetCoverUrls(int[] coverIds, string size)
            => [.. coverIds.Select(id => $"https://covers.openlibrary.org/b/id/{id}-{size}.jpg")];
    }
}
