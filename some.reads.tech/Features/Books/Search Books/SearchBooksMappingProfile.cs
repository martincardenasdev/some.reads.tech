using Mapster;
using some.reads.tech.Shared.Dto;

namespace some.reads.tech.Features.Books;

public class SearchBooksMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        TypeAdapterConfig<BookSearchResponse, BookDto>
            .NewConfig()
            .Map(dest => dest.CoverPics, src => new[] { SearchBooks.GetCoverUrl(src.CoverEditionKey, "L") })
            .Map(dest => dest.Key, src => src.CoverEditionKey);
    }
}