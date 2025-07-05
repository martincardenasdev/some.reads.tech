using Mapster;
using some.reads.tech.Common;
using some.reads.tech.Filters;
using some.reads.tech.Services;

namespace some.reads.tech.Features.Authors
{
    public static class SearchAuthors
    {
        public static void AddSearchAuthorsEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("authors/search", Handler).AddEndpointFilter<CacheEndpointFilter>();
        }

        private static async Task<IResult> Handler(string name, OpenLibraryService openLibraryService)
        { 
            var response = await openLibraryService.GetFromJsonAsync<OpenLibraryResponse<AuthorResponse>?>($"search/authors.json?q={name}");
            
            if (response is null || response.NumFound == 0) return Results.NotFound(new { message = "No authors found" });

            return Results.Ok(new { Count = response.NumFound, Authors = response.Docs.Adapt<AuthorDto[]>() });
        }

        public static string GetAuthorPictureUrl(string authorId, string size) => $"https://covers.openlibrary.org/a/olid/{authorId}-{size}.jpg";
    }
}
