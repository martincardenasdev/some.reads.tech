using Mapster;
using some.reads.tech.Common;
using some.reads.tech.Filters;
using some.reads.tech.Services;
using some.reads.tech.Shared.Dto;

namespace some.reads.tech.Features.Books;

public static class SearchBooks
{
    public static void AddSearchBooksEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("books/search", Handler).AddEndpointFilter<CacheEndpointFilter>();
    }

    private static async Task<IResult> Handler(string name, OpenLibraryService openLibraryService)
    {
        var response = await openLibraryService.GetFromJsonAsync<OpenLibraryResponse<BookSearchResponse>?>($"search.json?q={name}");

        if (response is null || response.NumFound == 0) return Results.NotFound(new { message = "No books found" });

        return Results.Ok(new { response.NumFound, response.Query, Docs = response.Docs.Adapt<BookDto[]>() });
    }

    public static string GetCoverUrl(string coverId, string size) => $"https://covers.openlibrary.org/b/olid/{coverId}-{size}.jpg";
}