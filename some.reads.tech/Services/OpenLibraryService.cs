using some.reads.tech.Common;
using some.reads.tech.Features.Authors;
using some.reads.tech.Features.Books;

namespace some.reads.tech.Services
{
    public sealed class OpenLibraryService(HttpClient httpClient)
    {
        public async Task<OpenLibraryResponse<BookResponse>?> SearchBooks(string name)
            => await httpClient.GetFromJsonAsync<OpenLibraryResponse<BookResponse>?>($"search.json?q={name}");

        public async Task<OpenLibraryResponse<AuthorResponse>?> SearchAuthors(string name)
            => await httpClient.GetFromJsonAsync<OpenLibraryResponse<AuthorResponse>?>($"search/authors.json?q={name}");
    }
}
