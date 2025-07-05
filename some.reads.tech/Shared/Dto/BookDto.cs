namespace some.reads.tech.Shared.Dto
{
    public record BookDto(
        string Title,
        string[] AuthorName,
        string? Key = null,
        string[]? Subjects = null,
        string? Description = null,
        string[]? CoverPics = null,
        string[]? Genres = null
        );
}
