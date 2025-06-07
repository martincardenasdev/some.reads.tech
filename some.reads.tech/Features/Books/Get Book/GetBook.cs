using Mapster;
using Microsoft.AspNetCore.Mvc;
using some.reads.tech.Filters;
using some.reads.tech.Services;
using some.reads.tech.Shared.Dto;

namespace some.reads.tech.Features.Books.Get_Book
{
    public static class GetBook
    {
        public static void AddGetBookEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("books/{*key}", Handler).AddEndpointFilter<CacheEndpointFilter>();
        }

        private static async Task<IResult> Handler(
            [FromRoute] string key,
            [FromServices] OpenLibraryService openLibraryService
            )
        { 
            var response = await openLibraryService.GetFromJsonAsync<BookResponse?>($"{key}.json");

            return response is null ? Results.NotFound(new { message = "Book not found" }) : Results.Ok(response.Adapt<BookDto>());
        }

        public static string[] GetCoverUrls(int[] coverIds, string size)
            => [.. coverIds.Select(id => $"https://covers.openlibrary.org/b/id/{id}-{size}.jpg")];
    }
}
