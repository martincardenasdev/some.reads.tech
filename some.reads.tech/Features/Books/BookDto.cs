namespace some.reads.tech.Features.Books
{
    public record BookDto(
        string Title,
        string[] AuthorNames,
        int? PublishYear = null,
        string[]? Subjects = null,
        string[]? SubjectPlaces = null,
        string[]? SubjectPeople = null,
        string[]? SubjectTimes = null,
        string? Description = null,
        string[]? CoverPics = null
        );
}
