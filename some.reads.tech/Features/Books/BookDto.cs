namespace some.reads.tech.Features.Books
{
    public record BookDto(
        string Title,
        string[] AuthorNames,
        string CoverPic,
        int PublishYear
        );
}
