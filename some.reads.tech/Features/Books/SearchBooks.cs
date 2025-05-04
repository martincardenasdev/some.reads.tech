using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using some.reads.tech.Services;

namespace some.reads.tech.Features.Books;

public static class SearchBooks
{
    public static void AddSearchBooksEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("books/search", Handler).RequireAuthorization();
    }

    private static async Task<IResult> Handler(
        [FromQuery] string name,
        [FromServices] OpenLibraryService openLibraryService,
        [FromServices] IMemoryCache memoryCache
        )
    {
        string cacheKey = $"books_search_{name}";

        var response = await memoryCache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
            return await openLibraryService.SearchBooks(name);
        });

        if (response is null || response.NumFound == 0) return Results.NotFound(new { message = "No books found" });

        var books = response.Docs
            .Select(doc => new BookDto
            (
                Title: doc.Title,
                AuthorNames: doc.AuthorName,
                CoverPic: GetCoverUrl(doc.CoverEditionKey, "L"),
                PublishYear: doc.FirstPublishYear
            )).ToArray();

        return Results.Ok(new { Count = response.NumFound, books });
    }

    private static string GetCoverUrl(string coverId, string size) => $"https://covers.openlibrary.org/b/olid/{coverId}-{size}.jpg";
}