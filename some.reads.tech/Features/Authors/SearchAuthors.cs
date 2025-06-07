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

            var authors = response.Docs
                .Select(doc => new AuthorDto(
                    Name: doc.Name,
                    Picture: GetAuthorPictureUrl(doc.Key, "L"),
                    TopSubjects: doc.TopSubjects,
                    TopWork: doc.TopWork,
                    BirthDate: doc.BirthDate
                )).ToArray();

            return Results.Ok(new { Count = response.NumFound, authors });
        }

        private static string GetAuthorPictureUrl(string authorId, string size) => $"https://covers.openlibrary.org/a/olid/{authorId}-{size}.jpg";
    }
}
