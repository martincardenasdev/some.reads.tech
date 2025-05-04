namespace some.reads.tech.Features.Authors
{
    public record AuthorDto(
        string Name,
        string Picture,
        string[] TopSubjects,
        string TopWork,
        string BirthDate
        );
}
