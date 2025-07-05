using Mapster;
using some.reads.tech.Shared.Dto;

namespace some.reads.tech.Features.Books.Get_Book
{
    public class GetBookMappingProfile : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            TypeAdapterConfig<BookResponse, BookDto>
                .NewConfig().Map(dest => dest.CoverPics, src => GetBook.GetCoverUrls(src.Covers, "L"));
        }
    }
}
