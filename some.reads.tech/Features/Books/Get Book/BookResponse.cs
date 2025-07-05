namespace some.reads.tech.Features.Books.Get_Book
{
    public record   BookResponse(
        string? Title,
        string? Description,
        string[]? Subjects,
        int[]? Covers,
        KeyValuePair<string>[]? Authors,
        KeyValuePair<string>[]? Languages,
        string[]? Genres);

    public record KeyValuePair<T>(
        T Key
    );
}
