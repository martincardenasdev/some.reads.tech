using Mapster;

namespace some.reads.tech.Features.Books.Get_Book
{
    public class BookMapperProfile : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            TypeAdapterConfig<BookResponse, BookDto>
                .NewConfig()
                .Map(dest => dest.AuthorNames, src => src.Authors == null ? Array.Empty<string>() : src.Authors.Select(a => a.Author.Key).ToArray())
                .Map(dest => dest.CoverPics, src => GetBook.GetCoverUrls(src.Covers, "L"))
                .Map(dest => dest.Description, src => src.Description == null ? null : src.Description.Value);
        }
    }
}
