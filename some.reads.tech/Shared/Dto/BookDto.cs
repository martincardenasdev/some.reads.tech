namespace some.reads.tech.Shared.Dto
{
    public record BookDto(
        string Title,
        string[] AuthorNames,
        string Query,
        string? Key = null,
        int? PublishYear = null,
        string[]? Subjects = null,
        string[]? SubjectPlaces = null,
        string[]? SubjectPeople = null,
        string[]? SubjectTimes = null,
        string? Description = null,
        string[]? CoverPics = null
        );
}
