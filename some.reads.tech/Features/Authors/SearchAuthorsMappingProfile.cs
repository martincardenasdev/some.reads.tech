using Mapster;

namespace some.reads.tech.Features.Authors;

public class SearchAuthorsMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        TypeAdapterConfig<AuthorResponse, AuthorDto>
            .NewConfig()
            .Map(dest => dest.Picture, src => SearchAuthors.GetAuthorPictureUrl(src.Key, "L"));
    }
}